using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DatabaseTHP.Class.Uniben;
using Newtonsoft.Json;

namespace API_QuanLyTHP.Controllers.Uniben;

public class UnibenService
{
	public async Task<DatabaseTHP.Class.Uniben.Uniben.UnibenToken?> GetTokenAsync(DatabaseTHP.Class.Uniben.Uniben.UnibenTokenRequest request, string link)
	{
		try
		{
			HttpClientHandler handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};
			HttpClient _httpClient = new HttpClient(handler);
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.32.0");
			_httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
			string json = System.Text.Json.JsonSerializer.Serialize(request);
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.PostAsync(link, content);
			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
			}
			DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse apiResponse = System.Text.Json.JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse>(await response.Content.ReadAsStringAsync());
			if (apiResponse == null || string.IsNullOrEmpty(apiResponse.payload.ToString()))
			{
				throw new Exception("Không nhận được dữ liệu hợp lệ từ API.");
			}
			return System.Text.Json.JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenToken>(apiResponse.payload.ToString());
		}
		catch (System.Text.Json.JsonException ex)
		{
			System.Text.Json.JsonException ex2 = ex;
			throw new Exception("Không thể phân tích dữ liệu TokenResponse.", ex2);
		}
	}

	public async Task<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse?> GetListSales(string token, string link)
	{
		try
		{
			HttpClientHandler handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};
			HttpClient _httpClient = new HttpClient(handler);
			new JsonSerializerSettings().NullValueHandling = NullValueHandling.Ignore;
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.32.0");
			_httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
			new StringContent(string.Empty, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.GetAsync(link);
			if (response == null)
			{
				return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse
				{
					status = 404,
					message = "NO_RESPONSE: Không nhận được phản hồi từ API."
				};
			}
			if (!response.IsSuccessStatusCode)
			{
				return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse
				{
					status = 404,
					message = "NO_RESPONSE: Không nhận được phản hồi từ API."
				};
			}
			return System.Text.Json.JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse>(await response.Content.ReadAsStringAsync());
		}
		catch (System.Text.Json.JsonException ex)
		{
			System.Text.Json.JsonException ex2 = ex;
			return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse
			{
				status = 404,
				message = "GetListInvoiced : Không thể phân tích dữ liệu GetListInvoiced." + ex2.Message
			};
		}
	}

	public async Task<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse?> GetOrderDetail(string token, string link)
	{
		try
		{
			HttpClientHandler handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};
			HttpClient _httpClient = new HttpClient(handler);
			new JsonSerializerSettings().NullValueHandling = NullValueHandling.Ignore;
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.32.0");
			_httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
			new StringContent(string.Empty, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.GetAsync(link);
			if (response == null)
			{
				return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse
				{
					status = 404,
					message = "NO_RESPONSE: Không nhận được phản hồi từ API."
				};
			}
			if (!response.IsSuccessStatusCode)
			{
			}
			return System.Text.Json.JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse>(await response.Content.ReadAsStringAsync());
		}
		catch (System.Text.Json.JsonException ex)
		{
			System.Text.Json.JsonException ex2 = ex;
			return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse
			{
				status = 404,
				message = "GetSale : Không thể phân tích dữ liệu GetSale." + ex2.Message
			};
		}
	}
}
