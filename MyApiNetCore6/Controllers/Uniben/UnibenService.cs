// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Controllers.Uniben.UnibenService
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace API_QuanLyTHP.Controllers.Uniben;

public class UnibenService
{
  public async Task<DatabaseTHP.Class.Uniben.Uniben.UnibenToken?> GetTokenAsync(
    DatabaseTHP.Class.Uniben.Uniben.UnibenTokenRequest request,
    string link)
  {
    DatabaseTHP.Class.Uniben.Uniben.UnibenToken tokenAsync;
    try
    {
      HttpClientHandler handler = new HttpClientHandler()
      {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
      };
      HttpClient _httpClient = new HttpClient((HttpMessageHandler) handler);
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.32.0");
      _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
      string json = System.Text.Json.JsonSerializer.Serialize<DatabaseTHP.Class.Uniben.Uniben.UnibenTokenRequest>(request);
      StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
      HttpResponseMessage response = await _httpClient.PostAsync(link, (HttpContent) content);
      if (!response.IsSuccessStatusCode)
        throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
      string rawResponse = await response.Content.ReadAsStringAsync();
      DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse apiResponse = System.Text.Json.JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse>(rawResponse);
      tokenAsync = apiResponse != null && !string.IsNullOrEmpty(apiResponse.payload.ToString()) ? System.Text.Json.JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenToken>(apiResponse.payload.ToString()) : throw new Exception("Không nhận được dữ liệu hợp lệ từ API.");
    }
    catch (System.Text.Json.JsonException ex)
    {
      throw new Exception("Không thể phân tích dữ liệu TokenResponse.", (Exception) ex);
    }
    return tokenAsync;
  }

  public async Task<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse?> GetListSales(
    string token,
    string link)
  {
    try
    {
      HttpClientHandler handler = new HttpClientHandler()
      {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
      };
      HttpClient _httpClient = new HttpClient((HttpMessageHandler) handler);
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      };
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.32.0");
      _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
      StringContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
      HttpResponseMessage response = await _httpClient.GetAsync(link);
      if (response == null)
        return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse()
        {
          status = 404,
          message = "NO_RESPONSE: Không nhận được phản hồi từ API."
        };
      if (!response.IsSuccessStatusCode)
        return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse()
        {
          status = 404,
          message = "NO_RESPONSE: Không nhận được phản hồi từ API."
        };
      string rawResponse = await response.Content.ReadAsStringAsync();
      DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse apiResponse = System.Text.Json.JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse>(rawResponse);
      return apiResponse;
    }
    catch (System.Text.Json.JsonException ex)
    {
      return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse()
      {
        status = 404,
        message = "GetListInvoiced : Không thể phân tích dữ liệu GetListInvoiced." + ex.Message
      };
    }
  }

  public async Task<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse?> GetOrderDetail(
    string token,
    string link)
  {
    try
    {
      HttpClientHandler handler = new HttpClientHandler()
      {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
      };
      HttpClient _httpClient = new HttpClient((HttpMessageHandler) handler);
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      };
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.32.0");
      _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
      StringContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
      HttpResponseMessage response = await _httpClient.GetAsync(link);
      if (response == null)
        return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse()
        {
          status = 404,
          message = "NO_RESPONSE: Không nhận được phản hồi từ API."
        };
      if (response.IsSuccessStatusCode)
        ;
      string rawResponse = await response.Content.ReadAsStringAsync();
      DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse apiResponse = System.Text.Json.JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse>(rawResponse);
      return apiResponse;
    }
    catch (System.Text.Json.JsonException ex)
    {
      return new DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse()
      {
        status = 404,
        message = "GetSale : Không thể phân tích dữ liệu GetSale." + ex.Message
      };
    }
  }
}
