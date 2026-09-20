using MeFriendApi.Domain.Dto;
using MeFriendApi.Domain.Dto.Helpers;
using MeFriendApi.Domain.DTO;
using MeFriendApi.Domain.Exceptions;
using MeFriendApi.Services.Infrastructure;
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
        private static readonly IReadOnlyCollection<string> AccessTokenScopes =
            Array.AsReadOnly([BusinessCentralDefaults.AccessTokenScope]);

        private static readonly JsonSerializerOptions DeserializationOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Lazy<IConfidentialClientApplication> _confidentialClientApplication;

        public D365CommonService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _confidentialClientApplication = new Lazy<IConfidentialClientApplication>(
                CreateConfidentialClientApplication,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public async Task<string> GetAccessToken()
        {
            var result = await _confidentialClientApplication.Value
                .AcquireTokenForClient(AccessTokenScopes)
                .ExecuteAsync();

            return result.AccessToken;
        }

        public async Task<List<T>> GetDataFromBc<T>(
            string apiPath,
            string? filter = "",
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V2,
            string? apiServiceName = null)
        {
            try
            {
                var token = await GetAccessToken();

                var client = CreateBusinessCentralClient(token);

                string url;
                if (!string.IsNullOrWhiteSpace(apiServiceName))
                {
                    url = _configuration[
                            BusinessCentralDefaults.ConfigurationKeys.ApiServiceUrl(apiServiceName)]
                        ?? throw new InternalException(
                            $"Business Central API URL configuration is missing for '{apiServiceName}'.");
                }
                else
                {
                    url = BuildApiUrl(apiPath, bcWebServiceProtocol);
                }

                url = AppendQueryString(url, filter);

                using var response = await client.GetAsync(url);

                await EnsureBusinessCentralSuccessAsync(response, $"GET {apiPath}");

                var json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);

                var root = document.RootElement;

                // Collection response
                if (root.TryGetProperty(BusinessCentralDefaults.ODataCollectionPropertyName, out _))
                {
                    var result = JsonSerializer.Deserialize<BcODataResponse<T>>(
                        json,
                        DeserializationOptions);

                    return result?.Value ?? new List<T>();
                }

                // Single object response
                var singleObject = JsonSerializer.Deserialize<T>(json, DeserializationOptions);

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

            var url = BuildApiUrl(apiPath, bcWebServiceProtocol);

            var json = JsonSerializer.Serialize(payload, SerializationOptions);
            using var content = new StringContent(
                json,
                Encoding.UTF8,
                BusinessCentralDefaults.JsonMediaType);

            using var response = await client.PostAsync(url, content);
            await EnsureBusinessCentralSuccessAsync(response, $"POST {apiPath}");

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(responseJson, DeserializationOptions);
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

                using var response = await client.GetAsync(url);
                await EnsureBusinessCentralSuccessAsync(response, $"GET ODataV4/{serviceName}");

                var json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                if (root.TryGetProperty(BusinessCentralDefaults.ODataCollectionPropertyName, out _))
                {
                    var result = JsonSerializer.Deserialize<BcODataResponse<T>>(
                        json,
                        DeserializationOptions);
                    return result?.Value ?? new List<T>();
                }

                var singleObject = JsonSerializer.Deserialize<T>(json, DeserializationOptions);

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

                var json = JsonSerializer.Serialize(payload, SerializationOptions);
                using var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    BusinessCentralDefaults.JsonMediaType);

                using var response = await client.PostAsync(url, content);
                await EnsureBusinessCentralSuccessAsync(response, $"POST ODataV4/{serviceName}");

                var responseJson = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseJson))
                    return default;

                return JsonSerializer.Deserialize<TResponse>(responseJson, DeserializationOptions);
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

            client.DefaultRequestHeaders.TryAddWithoutValidation(
                BusinessCentralDefaults.IfMatchHeaderName,
                BusinessCentralDefaults.MatchAnyEtag);
            var url = BuildApiUrl(apiPath, bcWebServiceProtocol);

            var json = JsonSerializer.Serialize(payload, SerializationOptions);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                BusinessCentralDefaults.JsonMediaType);

            using var request = new HttpRequestMessage(
                HttpMethod.Patch,
                url)
            {
                Content = content
            };

            using var response = await client.SendAsync(request);

            await EnsureBusinessCentralSuccessAsync(response, $"PATCH {apiPath}");

            // Some BC PATCH APIs return 204 NoContent
            if (response.Content == null)
                return default;

            var responseJson = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseJson))
                return default;

            return JsonSerializer.Deserialize<TResponse>(responseJson, DeserializationOptions);
        }
        public async Task DeleteDataFromBc(
            string apiPath,
            string? etag = null,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V1)
        {
            var token = await GetAccessToken();

            var client = CreateBusinessCentralClient(token);

            client.DefaultRequestHeaders.TryAddWithoutValidation(
                BusinessCentralDefaults.IfMatchHeaderName,
                BusinessCentralDefaults.MatchAnyEtag);

            var url = BuildApiUrl(apiPath, bcWebServiceProtocol);

            using var response = await client.DeleteAsync(url);

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

                var url = BuildApiUrl(
                    $"{filePathWithItemId}{BusinessCentralDefaults.ApiPaths.Attachments}",
                    bcWebServiceProtocol);

                await using var stream = file.OpenReadStream();

                using var content = new MultipartFormDataContent();

                var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(
                        string.IsNullOrWhiteSpace(file.ContentType)
                            ? BusinessCentralDefaults.BinaryMediaType
                            : file.ContentType);

                content.Add(
                    fileContent,
                    BusinessCentralDefaults.FileFormFieldName,
                    file.FileName);

                using var response = await client.PostAsync(url, content);

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
                    DeserializationOptions);
            }
            catch (Exception ex)
            {
                throw new InternalException(
                    $"Failed to upload attachment to Business Central: {ex.Message}",
                    ex);
            }
        }

        private HttpClient CreateBusinessCentralClient(string token)
        {
            var client = _httpClientFactory.CreateClient(BusinessCentralDefaults.HttpClientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        private IConfidentialClientApplication CreateConfidentialClientApplication() =>
            ConfidentialClientApplicationBuilder.Create(
                    _configuration[BusinessCentralDefaults.ConfigurationKeys.ClientId])
                .WithClientSecret(
                    _configuration[BusinessCentralDefaults.ConfigurationKeys.ClientSecret])
                .WithAuthority(
                    $"{BusinessCentralDefaults.AuthorityBaseUrl}/{_configuration[BusinessCentralDefaults.ConfigurationKeys.TenantId]}")
                .Build();

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

        private string BuildApiUrl(
            string apiPath,
            BcWebServiceProtocol? bcWebServiceProtocol)
        {
            var baseUrl = _configuration[BusinessCentralDefaults.ConfigurationKeys.BaseUrl];
            var protocolPath = GetProtocolPath(bcWebServiceProtocol);

            return $"{baseUrl}/{protocolPath}{apiPath}";
        }

        private string GetProtocolPath(BcWebServiceProtocol? bcWebServiceProtocol)
        {
            var companyId = _configuration[BusinessCentralDefaults.ConfigurationKeys.CompanyId];

            return bcWebServiceProtocol switch
            {
                BcWebServiceProtocol.V2 => BusinessCentralDefaults.ProtocolPaths.V2,
                BcWebServiceProtocol.ODataV4 => string.Format(
                    BusinessCentralDefaults.ProtocolPaths.ODataV4,
                    _configuration[BusinessCentralDefaults.ConfigurationKeys.CompanyName]),
                BcWebServiceProtocol.V1 => string.Format(
                    BusinessCentralDefaults.ProtocolPaths.V1,
                    companyId),
                BcWebServiceProtocol.ItemMasterV1 or BcWebServiceProtocol.CustomerMasterV1 =>
                    string.Format(BusinessCentralDefaults.ProtocolPaths.MefriendV1, companyId),
                _ => throw new ArgumentOutOfRangeException(nameof(bcWebServiceProtocol))
            };
        }

        private string BuildNamedODataServiceUrl(string serviceName, string? queryString = null)
        {
            var configuredServiceUrl = _configuration[
                BusinessCentralDefaults.ConfigurationKeys.ODataServiceUrl(serviceName)];

            if (!string.IsNullOrWhiteSpace(configuredServiceUrl))
            {
                return AppendQueryString(configuredServiceUrl, queryString);
            }

            var baseUrl = _configuration[BusinessCentralDefaults.ConfigurationKeys.BaseUrl]?.TrimEnd('/');
            var companyName =
                _configuration[BusinessCentralDefaults.ConfigurationKeys.CompanyName];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InternalException(
                    $"{BusinessCentralDefaults.ConfigurationKeys.BaseUrl} configuration value is missing.");

            if (string.IsNullOrWhiteSpace(companyName))
                throw new InternalException(
                    $"{BusinessCentralDefaults.ConfigurationKeys.CompanyName} configuration value is missing.");

            var encodedCompanyName = Uri.EscapeDataString(Uri.UnescapeDataString(companyName));
            var url = $"{baseUrl}/ODataV4/{serviceName}?company={encodedCompanyName}";

            return AppendQueryString(url, queryString);
        }

        private static string AppendQueryString(string url, string? queryString = null)
        {
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                var normalizedQuery = queryString.TrimStart('?', '&');

                if (!string.IsNullOrWhiteSpace(normalizedQuery))
                {
                    var separator = url.Contains('?') ? "&" : "?";
                    url += $"{separator}{normalizedQuery}";
                }
            }

            return url;
        }
    }
}
