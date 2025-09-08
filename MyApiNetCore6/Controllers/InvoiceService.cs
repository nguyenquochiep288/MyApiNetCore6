using DatabaseTHP.Class;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace API_QuanLyTHP.Controllers
{
    public class InvoiceService
    {
        public async Task<ApiResponseInvoiced?> SubmitInvoiceAsync(List<InvoiceMaster> invoices, string token, string taxcode, string link)
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                var json = JsonSerializer.Serialize(invoices);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("taxcode", taxcode);

                var response = await _httpClient.PostAsync(link, content);
                if (!response.IsSuccessStatusCode)
                {
                    // Ghi log lỗi hoặc xử lý theo yêu cầu
                    throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
                }
                var rawResponse = await response.Content.ReadAsStringAsync();

                // Giải mã lớp ngoài
                var apiResponse = JsonSerializer.Deserialize<ApiResponseInvoiced>(rawResponse);
                return apiResponse;
            }
            catch (JsonException ex)
            {
                throw new Exception("Không thể phân tích dữ liệu TokenResponse.", ex);
            }
        }

        public async Task<TokenData?> GetTokenAsync(TokenRequest request, string link)
        {        
            try
            {
                HttpClient _httpClient = new HttpClient();
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(link, content);
                if (!response.IsSuccessStatusCode)
                {
                    // Ghi log lỗi hoặc xử lý theo yêu cầu
                    throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
                }
                var rawResponse = await response.Content.ReadAsStringAsync();

                // Giải mã lớp ngoài
                var apiResponse = JsonSerializer.Deserialize<ApiResponseInvoiced>(rawResponse);
                if (apiResponse == null || string.IsNullOrEmpty(apiResponse.Data))
                {
                    throw new Exception("Không nhận được dữ liệu hợp lệ từ API.");
                }
                // Giải mã chuỗi JSON trong trường Data
                var token = JsonSerializer.Deserialize<TokenData>(apiResponse.Data);
                return token;
            }
            catch (JsonException ex)
            {
                throw new Exception("Không thể phân tích dữ liệu TokenResponse.", ex);
            }
        }

        public async Task<ApiResponseInvoiced?> SubmitTemplateAsync(TokenRequest request, string link)
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(link, content);
                if (!response.IsSuccessStatusCode)
                {
                    // Ghi log lỗi hoặc xử lý theo yêu cầu
                    throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
                }
                var rawResponse = await response.Content.ReadAsStringAsync();

                // Giải mã lớp ngoài
                var apiResponse = JsonSerializer.Deserialize<ApiResponseInvoiced>(rawResponse);
                return apiResponse;
            }
            catch (JsonException ex)
            {
                throw new Exception("Không thể phân tích dữ liệu TokenResponse.", ex);
            }
        }
    }
}
