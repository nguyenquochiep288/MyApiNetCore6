using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DatabaseTHP.Class.Misa;
using Newtonsoft.Json;

namespace API_QuanLyTHP.Controllers.Misa;

public class InvoiceService
{
	public async Task<MisaApiResponseInvoiced?> InsertInvoiceAsync(List<MisaInvoiceMaster> invoices, string token, string taxcode, string link)
	{
		try
		{
			HttpClient _httpClient = new HttpClient();
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
			string json = JsonConvert.SerializeObject(invoices, settings);
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
			_httpClient.DefaultRequestHeaders.Add("taxcode", taxcode);
			HttpResponseMessage response = await _httpClient.PostAsync(link, content);
			if (!response.IsSuccessStatusCode)
			{
			}
			return System.Text.Json.JsonSerializer.Deserialize<MisaApiResponseInvoiced>(await response.Content.ReadAsStringAsync());
		}
		catch (System.Text.Json.JsonException ex)
		{
			System.Text.Json.JsonException ex2 = ex;
			return new MisaApiResponseInvoiced
			{
				success = false,
				error = "InsertInvoiceAsync",
				error_description = "Không thể phân tích dữ liệu InsertInvoiceAsync." + ex2.Message
			};
		}
	}

	public async Task<MisaApiResponseInvoiced?> GetListInvoiced(List<string> invoices, string token, string taxcode, string link)
	{
		try
		{
			HttpClient _httpClient = new HttpClient();
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
			string json = JsonConvert.SerializeObject(invoices, settings);
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
			_httpClient.DefaultRequestHeaders.Add("taxcode", taxcode);
			HttpResponseMessage response = await _httpClient.PostAsync(link, content);
			if (response == null)
			{
				return new MisaApiResponseInvoiced
				{
					success = false,
					error = "NO_RESPONSE",
					error_description = "Không nhận được phản hồi từ API."
				};
			}
			if (!response.IsSuccessStatusCode)
			{
			}
			return System.Text.Json.JsonSerializer.Deserialize<MisaApiResponseInvoiced>(await response.Content.ReadAsStringAsync());
		}
		catch (System.Text.Json.JsonException ex)
		{
			System.Text.Json.JsonException ex2 = ex;
			return new MisaApiResponseInvoiced
			{
				success = false,
				error = "GetListInvoiced",
				error_description = "Không thể phân tích dữ liệu GetListInvoiced." + ex2.Message
			};
		}
	}

	public async Task<MisaTokenData?> GetTokenAsync(MisaTokenRequest request, string link)
	{
		try
		{
			HttpClient _httpClient = new HttpClient();
			string json = System.Text.Json.JsonSerializer.Serialize(request);
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.PostAsync(link, content);
			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
			}
			MisaApiResponseInvoiced apiResponse = System.Text.Json.JsonSerializer.Deserialize<MisaApiResponseInvoiced>(await response.Content.ReadAsStringAsync());
			if (apiResponse == null || string.IsNullOrEmpty(apiResponse.data))
			{
				throw new Exception("Không nhận được dữ liệu hợp lệ từ API.");
			}
			return System.Text.Json.JsonSerializer.Deserialize<MisaTokenData>(apiResponse.data);
		}
		catch (System.Text.Json.JsonException ex)
		{
			System.Text.Json.JsonException ex2 = ex;
			throw new Exception("Không thể phân tích dữ liệu TokenResponse.", ex2);
		}
	}

	public async Task<MisaApiResponseInvoiced?> GetTemplateAsync(MisaTokenRequest request, string link)
	{
		try
		{
			HttpClient _httpClient = new HttpClient();
			string json = System.Text.Json.JsonSerializer.Serialize(request);
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.PostAsync(link, content);
			if (response == null)
			{
				return new MisaApiResponseInvoiced
				{
					success = false,
					error = "NO_RESPONSE",
					error_description = "Không nhận được phản hồi từ API."
				};
			}
			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
			}
			return System.Text.Json.JsonSerializer.Deserialize<MisaApiResponseInvoiced>(await response.Content.ReadAsStringAsync());
		}
		catch (System.Text.Json.JsonException ex)
		{
			System.Text.Json.JsonException ex2 = ex;
			return new MisaApiResponseInvoiced
			{
				success = false,
				error = "GetTemplateAsync",
				error_description = "Không thể phân tích dữ liệu GetTemplateAsync." + ex2.Message
			};
		}
	}
}
