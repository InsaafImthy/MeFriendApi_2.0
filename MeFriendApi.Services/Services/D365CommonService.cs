using MeFriendApi.Domain.Dto;
using MeFriendApi.Domain.Dto.Helpers;
using MeFriendApi.Domain.DTO;
using MeFriendApi.Domain.Exceptions;
using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class D365CommonService : ID365CommonService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public D365CommonService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> GetAccessToken()
        {
            var app = ConfidentialClientApplicationBuilder.Create(_configuration["AzureAd:ClientId"])
                .WithClientSecret(_configuration["AzureAd:ClientSecret"])
                .WithAuthority($"https://login.microsoftonline.com/{_configuration["AzureAd:TenantId"]}")
                .Build();

            var scopes = new[] { "https://api.businesscentral.dynamics.com/.default" };

            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            return result.AccessToken;
        }

        public async Task<List<T>> GetDataFromBc<T>(
            string apiPath,
            string? filter = "",
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V2)
        {
            try
            {
                var token = await GetAccessToken();

                var client = CreateBusinessCentralClient(token);

                var protocolPath = bcWebServiceProtocol switch
                {
                    BcWebServiceProtocol.V2 => "api/v2.0",
                    BcWebServiceProtocol.ODataV4 => $"ODataV4/Company('{_configuration["CompanyInfo:CompanyName"]}')",
                    BcWebServiceProtocol.V1 => $"api/CVT/CVTGroup/v1.0/Companies({_configuration["CompanyInfo:CompanyId"]})",
                    _ => throw new ArgumentOutOfRangeException(nameof(bcWebServiceProtocol))
                };

                var url = $"{_configuration["AzureAd:BaseUrl"]}/{protocolPath}{apiPath}";

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    url += filter;
                }

                var response = await client.GetAsync(url);

                await EnsureBusinessCentralSuccessAsync(response, $"GET {apiPath}");

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                using var document = JsonDocument.Parse(json);

                var root = document.RootElement;

                // Collection response
                if (root.TryGetProperty("value", out var valueElement))
                {
                    var result =
                        JsonSerializer.Deserialize<BcODataResponse<T>>(json, options);

                    return result?.Value ?? new List<T>();
                }

                // Single object response
                var singleObject =
                    JsonSerializer.Deserialize<T>(json, options);

                return singleObject != null
                    ? new List<T> { singleObject }
                    : new List<T>();
            }
            catch (Exception ex)
            {
                throw new InternalException($"Failed to get response: {ex.Message}", ex);
            }
        }
        
        public async Task<TResponse?> PostDataToBc<TRequest, TResponse>(
            string apiPath,
            TRequest payload,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1)
        {
            var token = await GetAccessToken();

            var client = CreateBusinessCentralClient(token);

            var protocolPath = bcWebServiceProtocol switch
            {
                BcWebServiceProtocol.V2 => "api/v2.0",
                BcWebServiceProtocol.ODataV4 => $"ODataV4/Company('{_configuration["CompanyInfo:CompanyName"]}')",
                BcWebServiceProtocol.V1 => $"api/CVT/CVTGroup/v1.0/Companies({_configuration["CompanyInfo:CompanyId"]})",
                _ => throw new ArgumentOutOfRangeException(nameof(bcWebServiceProtocol))
            };

            var url = $"{_configuration["AzureAd:BaseUrl"]}/{protocolPath}{apiPath}";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(payload, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            await EnsureBusinessCentralSuccessAsync(response, $"POST {apiPath}");

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<List<T>> GetFromODataServiceAsync<T>(
            string serviceName,
            string? queryString = null)
        {
            try
            {
                var token = await GetAccessToken();

                var client = CreateBusinessCentralClient(token);

                var url = BuildNamedODataServiceUrl(serviceName, queryString);

                var response = await client.GetAsync(url);
                await EnsureBusinessCentralSuccessAsync(response, $"GET ODataV4/{serviceName}");

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                if (root.TryGetProperty("value", out _))
                {
                    var result = JsonSerializer.Deserialize<BcODataResponse<T>>(json, options);
                    return result?.Value ?? new List<T>();
                }

                var singleObject = JsonSerializer.Deserialize<T>(json, options);

                return singleObject != null
                    ? new List<T> { singleObject }
                    : new List<T>();
            }
            catch (Exception ex)
            {
                throw new InternalException(
                    $"Failed to get response from Business Central OData service '{serviceName}': {ex.Message}",
                    ex);
            }
        }

        public async Task<TResponse?> PostToODataServiceAsync<TRequest, TResponse>(
            string serviceName,
            TRequest payload,
            string? queryString = null)
        {
            try
            {
                var token = await GetAccessToken();

                var client = CreateBusinessCentralClient(token);

                var url = BuildNamedODataServiceUrl(serviceName, queryString);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(payload, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                await EnsureBusinessCentralSuccessAsync(response, $"POST ODataV4/{serviceName}");

                var responseJson = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseJson))
                    return default;

                return JsonSerializer.Deserialize<TResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                throw new InternalException(
                    $"Failed to post to Business Central OData service '{serviceName}': {ex.Message}",
                    ex);
            }
        }

        public async Task<TResponse?> PatchDataToBc<TRequest, TResponse>(
            string apiPath,
            TRequest payload,
            string? etag = null,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1)
        {
            var token = await GetAccessToken();

            var client = CreateBusinessCentralClient(token);

            var protocolPath = bcWebServiceProtocol switch
            {
                BcWebServiceProtocol.V2 => "api/v2.0",
                BcWebServiceProtocol.ODataV4 => $"ODataV4/Company('{_configuration["CompanyInfo:CompanyName"]}')",
                BcWebServiceProtocol.V1 => $"api/CVT/CVTGroup/v1.0/Companies({_configuration["CompanyInfo:CompanyId"]})",
                _ => throw new ArgumentOutOfRangeException(nameof(bcWebServiceProtocol))
            };

            client.DefaultRequestHeaders.TryAddWithoutValidation(
                                "If-Match",
                                "*");
            var url = $"{_configuration["AzureAd:BaseUrl"]}/{protocolPath}{apiPath}";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var json = JsonSerializer.Serialize(payload, options);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Patch,
                url)
            {
                Content = content
            };

            var response = await client.SendAsync(request);

            await EnsureBusinessCentralSuccessAsync(response, $"PATCH {apiPath}");

            // Some BC PATCH APIs return 204 NoContent
            if (response.Content == null)
                return default;

            var responseJson = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseJson))
                return default;

            return JsonSerializer.Deserialize<TResponse>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        public async Task DeleteDataFromBc(
            string apiPath,
            string? etag = null,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1)
        {
            var token = await GetAccessToken();

            var client = CreateBusinessCentralClient(token);

            var protocolPath = bcWebServiceProtocol switch
            {
                BcWebServiceProtocol.V2 => "api/v2.0",
                BcWebServiceProtocol.ODataV4 => $"ODataV4/Company('{_configuration["CompanyInfo:CompanyName"]}')",
                BcWebServiceProtocol.V1 => $"api/CVT/CVTGroup/v1.0/Companies({_configuration["CompanyInfo:CompanyId"]})",
                _ => throw new ArgumentOutOfRangeException(nameof(bcWebServiceProtocol))
            };

            client.DefaultRequestHeaders.TryAddWithoutValidation(
                                "If-Match",
                                "*");

            var url = $"{_configuration["AzureAd:BaseUrl"]}/{protocolPath}{apiPath}";

            var response = await client.DeleteAsync(url);

            await EnsureBusinessCentralSuccessAsync(response, $"DELETE {apiPath}");
        }

        public async Task<BcAttachmentResponseDto?> UploadAttachmentsToBcAsync(
            string filePathWithItemId,
            IFormFile file,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new BadRequestException("File is required.");

                var token = await GetAccessToken();

                var client = CreateBusinessCentralClient(token);

                var protocolPath = bcWebServiceProtocol switch
                {
                    BcWebServiceProtocol.V2 =>
                        "api/v2.0",

                    BcWebServiceProtocol.ODataV4 =>
                        $"ODataV4/Company('{_configuration["CompanyInfo:CompanyName"]}')",

                    BcWebServiceProtocol.V1 =>
                        $"api/CVT/CVTGroup/v1.0/Companies({_configuration["CompanyInfo:CompanyId"]})",

                    _ => throw new ArgumentOutOfRangeException(nameof(bcWebServiceProtocol))
                };

                var url =
                    $"{_configuration["AzureAd:BaseUrl"]}/{protocolPath}{filePathWithItemId}/Attachments";

                await using var stream = file.OpenReadStream();

                using var content = new MultipartFormDataContent();

                var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(
                        string.IsNullOrWhiteSpace(file.ContentType)
                            ? "application/octet-stream"
                            : file.ContentType);

                content.Add(
                    fileContent,
                    "file",
                    file.FileName);

                var response = await client.PostAsync(url, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new InternalException(
                        $"BC attachment upload failed. Status: {(int)response.StatusCode} - {responseContent}");
                }

                if (string.IsNullOrWhiteSpace(responseContent))
                    return null;

                return JsonSerializer.Deserialize<BcAttachmentResponseDto>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (Exception ex)
            {
                throw new InternalException(
                    $"Failed to upload attachment to Business Central: {ex.Message}");
            }
        }

        private HttpClient CreateBusinessCentralClient(string token)
        {
            var client = _httpClientFactory.CreateClient(nameof(D365CommonService));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        private static async Task EnsureBusinessCentralSuccessAsync(
            HttpResponseMessage response,
            string operation)
        {
            if (response.IsSuccessStatusCode)
                return;

            var responseContent = response.Content == null
                ? string.Empty
                : await response.Content.ReadAsStringAsync();

            var message =
                $"Business Central request failed for {operation}. " +
                $"Status: {(int)response.StatusCode} {response.ReasonPhrase}.";

            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                message += $" Response: {responseContent}";
            }

            throw new InternalException(message);
        }

        private string BuildNamedODataServiceUrl(string serviceName, string? queryString = null)
        {
            var baseUrl = _configuration["AzureAd:BaseUrl"]?.TrimEnd('/');
            var companyName = _configuration["CompanyInfo:CompanyName"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InternalException("AzureAd:BaseUrl configuration value is missing.");

            if (string.IsNullOrWhiteSpace(companyName))
                throw new InternalException("CompanyInfo:CompanyName configuration value is missing.");

            var encodedCompanyName = Uri.EscapeDataString(companyName);
            var url = $"{baseUrl}/ODataV4/{serviceName}?company={encodedCompanyName}";

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                var normalizedQuery = queryString.TrimStart('?', '&');

                if (!string.IsNullOrWhiteSpace(normalizedQuery))
                {
                    url += $"&{normalizedQuery}";
                }
            }

            return url;
        }
    }
}
