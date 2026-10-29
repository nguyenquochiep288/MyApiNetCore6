using System.Globalization;

namespace MyApiNetCore6.Data;

/// <summary>
/// Evaluates the arithmetic expressions used by price formulas.
/// </summary>
public sealed class PriceFormulaCalculator
{
    private readonly string _text;
    private readonly IReadOnlyDictionary<string, decimal> _values;
    private int _position;

    private PriceFormulaCalculator(string text, IReadOnlyDictionary<string, decimal> values)
    {
        _text = text ?? throw new ArgumentNullException(nameof(text));
        _values = values;
    }

    public static decimal Evaluate(string expression, IReadOnlyDictionary<string, decimal> values)
    {
        var calculator = new PriceFormulaCalculator(expression, values);
        var result = calculator.ParseExpression();
        calculator.SkipWhitespace();

        if (calculator._position != calculator._text.Length)
            throw new FormatException($"Ký tự không hợp lệ tại vị trí {calculator._position + 1}.");

        return result;
    }

    private decimal ParseExpression()
    {
        var result = ParseTerm();

        while (true)
        {
            SkipWhitespace();
            if (Take('+'))
            {
                result += ParseTerm();
            }
            else if (Take('-'))
            {
                result -= ParseTerm();
            }
            else
            {
                return result;
            }
        }
    }

    private decimal ParseTerm()
    {
        var result = ParseUnary();

        while (true)
        {
            SkipWhitespace();
            if (Take('*') || Take('×'))
            {
                result *= ParseUnary();
            }
            else if (Take('/') || Take('÷'))
            {
                var divisor = ParseUnary();
                if (divisor == decimal.Zero)
                    throw new DivideByZeroException("Không thể chia cho 0.");

                result /= divisor;
            }
            else
            {
                return result;
            }
        }
    }

    private decimal ParseUnary()
    {
        SkipWhitespace();
        if (Take('+'))
            return ParseUnary();
        if (Take('-'))
            return -ParseUnary();

        return ParsePrimary();
    }

    private decimal ParsePrimary()
    {
        SkipWhitespace();

        if (Take('('))
        {
            var result = ParseExpression();
            SkipWhitespace();
            if (!Take(')'))
                throw new FormatException("Thiếu dấu đóng ngoặc.");

            return result;
        }

        if (_position >= _text.Length)
            throw new FormatException("Công thức chưa hoàn chỉnh.");

        var current = _text[_position];
        if (char.IsDigit(current) || current == '.')
            return ParseNumber();
        if (char.IsLetter(current) || current == '_')
            return ParseVariable();

        throw new FormatException($"Ký tự không hợp lệ '{current}'.");
    }

    private decimal ParseNumber()
    {
        var start = _position;
        var hasDecimalPoint = false;

        while (_position < _text.Length)
        {
            var current = _text[_position];
            if (char.IsDigit(current))
            {
                _position++;
            }
            else if (current == '.' && !hasDecimalPoint)
            {
                hasDecimalPoint = true;
                _position++;
            }
            else
            {
                break;
            }
        }

        var numberText = _text.Substring(start, _position - start);
        if (!decimal.TryParse(numberText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value))
            throw new FormatException("Số trong công thức không hợp lệ.");

        return value;
    }

    private decimal ParseVariable()
    {
        var start = _position++;
        while (_position < _text.Length &&
               (char.IsLetterOrDigit(_text[_position]) || _text[_position] == '_'))
        {
            _position++;
        }

        var name = _text.Substring(start, _position - start);
        if (_values.TryGetValue(name, out var value))
            return value;

        throw new FormatException($"Chưa khai báo thành phần '{name}'.");
    }

    private bool Take(char value)
    {
        if (_position >= _text.Length || _text[_position] != value)
            return false;

        _position++;
        return true;
    }

    private void SkipWhitespace()
    {
        while (_position < _text.Length && char.IsWhiteSpace(_text[_position]))
            _position++;
    }
}
