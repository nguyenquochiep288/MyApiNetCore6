// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Controllers.Misa.InvoiceService
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP.Class.Misa;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace API_QuanLyTHP.Controllers.Misa;

public class InvoiceService
{
  public async Task<MisaApiResponseInvoiced?> InsertInvoiceAsync(
    List<MisaInvoiceMaster> invoices,
    string token,
    string taxcode,
    string link)
  {
    try
    {
      HttpClient _httpClient = new HttpClient();
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      };
      string json = JsonConvert.SerializeObject((object) invoices, settings);
      StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
      _httpClient.DefaultRequestHeaders.Add(nameof (taxcode), taxcode);
      HttpResponseMessage response = await _httpClient.PostAsync(link, (HttpContent) content);
      if (response.IsSuccessStatusCode)
        ;
      string rawResponse = await response.Content.ReadAsStringAsync();
      MisaApiResponseInvoiced apiResponse = System.Text.Json.JsonSerializer.Deserialize<MisaApiResponseInvoiced>(rawResponse);
      return apiResponse;
    }
    catch (System.Text.Json.JsonException ex)
    {
      return new MisaApiResponseInvoiced()
      {
        success = false,
        error = nameof (InsertInvoiceAsync),
        error_description = "Không thể phân tích dữ liệu InsertInvoiceAsync." + ex.Message
      };
    }
  }

  public async Task<MisaApiResponseInvoiced?> GetListInvoiced(
    List<string> invoices,
    string token,
    string taxcode,
    string link)
  {
    try
    {
      HttpClient _httpClient = new HttpClient();
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      };
      string json = JsonConvert.SerializeObject((object) invoices, settings);
      StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
      _httpClient.DefaultRequestHeaders.Add(nameof (taxcode), taxcode);
      HttpResponseMessage response = await _httpClient.PostAsync(link, (HttpContent) content);
      if (response == null)
        return new MisaApiResponseInvoiced()
        {
          success = false,
          error = "NO_RESPONSE",
          error_description = "Không nhận được phản hồi từ API."
        };
      if (response.IsSuccessStatusCode)
        ;
      string rawResponse = await response.Content.ReadAsStringAsync();
      MisaApiResponseInvoiced apiResponse = System.Text.Json.JsonSerializer.Deserialize<MisaApiResponseInvoiced>(rawResponse);
      return apiResponse;
    }
    catch (System.Text.Json.JsonException ex)
    {
      return new MisaApiResponseInvoiced()
      {
        success = false,
        error = nameof (GetListInvoiced),
        error_description = "Không thể phân tích dữ liệu GetListInvoiced." + ex.Message
      };
    }
  }

  public async Task<MisaTokenData?> GetTokenAsync(MisaTokenRequest request, string link)
  {
    MisaTokenData tokenAsync;
    try
    {
      HttpClient _httpClient = new HttpClient();
      string json = System.Text.Json.JsonSerializer.Serialize<MisaTokenRequest>(request);
      StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
      HttpResponseMessage response = await _httpClient.PostAsync(link, (HttpContent) content);
      if (!response.IsSuccessStatusCode)
        throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
      string rawResponse = await response.Content.ReadAsStringAsync();
      MisaApiResponseInvoiced apiResponse = System.Text.Json.JsonSerializer.Deserialize<MisaApiResponseInvoiced>(rawResponse);
      tokenAsync = apiResponse != null && !string.IsNullOrEmpty(apiResponse.data) ? System.Text.Json.JsonSerializer.Deserialize<MisaTokenData>(apiResponse.data) : throw new Exception("Không nhận được dữ liệu hợp lệ từ API.");
    }
    catch (System.Text.Json.JsonException ex)
    {
      throw new Exception("Không thể phân tích dữ liệu TokenResponse.", (Exception) ex);
    }
    return tokenAsync;
  }

  public async Task<MisaApiResponseInvoiced?> GetTemplateAsync(
    MisaTokenRequest request,
    string link)
  {
    try
    {
      HttpClient _httpClient = new HttpClient();
      string json = System.Text.Json.JsonSerializer.Serialize<MisaTokenRequest>(request);
      StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
      HttpResponseMessage response = await _httpClient.PostAsync(link, (HttpContent) content);
      if (response == null)
        return new MisaApiResponseInvoiced()
        {
          success = false,
          error = "NO_RESPONSE",
          error_description = "Không nhận được phản hồi từ API."
        };
      if (!response.IsSuccessStatusCode)
        throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
      string rawResponse = await response.Content.ReadAsStringAsync();
      MisaApiResponseInvoiced apiResponse = System.Text.Json.JsonSerializer.Deserialize<MisaApiResponseInvoiced>(rawResponse);
      return apiResponse;
    }
    catch (System.Text.Json.JsonException ex)
    {
      return new MisaApiResponseInvoiced()
      {
        success = false,
        error = nameof (GetTemplateAsync),
        error_description = "Không thể phân tích dữ liệu GetTemplateAsync." + ex.Message
      };
    }
  }
}
