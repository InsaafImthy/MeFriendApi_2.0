using MeFriendApi.Domain.Dto.Helpers;
using MeFriendApi.Domain.Dto.Paging;
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
        private readonly IBusinessCentralCompanyContext _companyContext;
        private readonly IBusinessCentralContinuationTokenService _continuationTokenService;
        private readonly Lazy<IConfidentialClientApplication> _confidentialClientApplication;

        public D365CommonService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IBusinessCentralCompanyContext companyContext,
            IBusinessCentralContinuationTokenService continuationTokenService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _companyContext = companyContext;
            _continuationTokenService = continuationTokenService;
            _confidentialClientApplication = new Lazy<IConfidentialClientApplication>(
                CreateConfidentialClientApplication,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public virtual async Task<string> GetAccessToken()
        {
            var result = await _confidentialClientApplication.Value
                .AcquireTokenForClient(AccessTokenScopes)
                .ExecuteAsync();

            return result.AccessToken;
        }

        public Task<PagedResult<T>> GetPagedDataFromBc<T>(
            string apiPath,
            int? pageSize = null,
            string? continuationToken = null,
            string? queryString = null,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V2)
        {
            var initialUrl = BuildApiUrl(apiPath, bcWebServiceProtocol);
            return GetPagedDataCoreAsync<T>(
                initialUrl,
                apiPath,
                bcWebServiceProtocol,
                pageSize,
                continuationToken,
                queryString,
                $"GET {apiPath}");
        }

        public async Task<T?> GetSingleDataFromBc<T>(
            string apiPath,
            string? queryString = null,
            BcWebServiceProtocol? bcWebServiceProtocol = BcWebServiceProtocol.V2)
        {
            try
            {
                var url = AppendQueryString(BuildApiUrl(apiPath, bcWebServiceProtocol), queryString);
                var token = await GetAccessToken();
                var client = CreateBusinessCentralClient(token);

                using var response = await client.GetAsync(url);
                await EnsureBusinessCentralSuccessAsync(response, $"GET {apiPath}");

                var json = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(json);

                if (document.RootElement.TryGetProperty(
                        BusinessCentralDefaults.ODataCollectionPropertyName,
                        out _))
                {
                    var collection = JsonSerializer.Deserialize<BcODataResponse<T>>(
                        json,
                        DeserializationOptions);
                    return collection == null ? default : collection.Value.FirstOrDefault();
                }

                return JsonSerializer.Deserialize<T>(json, DeserializationOptions);
            }
            catch (BadRequestException)
            {
                throw;
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
            var url = BuildApiUrl(apiPath, bcWebServiceProtocol);

            var token = await GetAccessToken();

            var client = CreateBusinessCentralClient(token);

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

        public Task<PagedResult<T>> GetPagedFromODataServiceAsync<T>(
            string serviceName,
            int? pageSize = null,
            string? continuationToken = null,
            string? queryString = null)
        {
            var initialUrl = BuildNamedODataServiceUrl(serviceName);
            return GetPagedDataCoreAsync<T>(
                initialUrl,
                $"ODataV4/{serviceName}",
                BcWebServiceProtocol.ODataV4,
                pageSize,
                continuationToken,
                queryString,
                $"GET ODataV4/{serviceName}");
        }

        public async Task<TResponse?> PostToODataServiceAsync<TRequest, TResponse>(
            string serviceName,
            TRequest payload,
            string? queryString = null)
        {
            try
            {
                var url = BuildNamedODataServiceUrl(serviceName, queryString);

                var token = await GetAccessToken();

                var client = CreateBusinessCentralClient(token);

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
            catch (BadRequestException)
            {
                throw;
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
            var url = BuildApiUrl(apiPath, bcWebServiceProtocol);

            var token = await GetAccessToken();

            var client = CreateBusinessCentralClient(token);

            client.DefaultRequestHeaders.TryAddWithoutValidation(
                BusinessCentralDefaults.IfMatchHeaderName,
                BusinessCentralDefaults.MatchAnyEtag);
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
            var url = BuildApiUrl(apiPath, bcWebServiceProtocol);

            var token = await GetAccessToken();

            var client = CreateBusinessCentralClient(token);

            client.DefaultRequestHeaders.TryAddWithoutValidation(
                BusinessCentralDefaults.IfMatchHeaderName,
                BusinessCentralDefaults.MatchAnyEtag);

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

                var url = BuildApiUrl(
                    $"{filePathWithItemId}{BusinessCentralDefaults.ApiPaths.Attachments}",
                    bcWebServiceProtocol);

                var token = await GetAccessToken();

                var client = CreateBusinessCentralClient(token);

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
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InternalException(
                    $"Failed to upload attachment to Business Central: {ex.Message}",
                    ex);
            }
        }

        private async Task<PagedResult<T>> GetPagedDataCoreAsync<T>(
            string initialUrl,
            string resourceKey,
            BcWebServiceProtocol? protocol,
            int? requestedPageSize,
            string? continuationToken,
            string? queryString,
            string operation)
        {
            try
            {
                var normalizedQuery = NormalizeQueryString(queryString);
                var collectionUrl = AppendQueryString(initialUrl, normalizedQuery);
                var effectivePageSize = ValidatePageSize(requestedPageSize);
                string requestUrl;

                if (string.IsNullOrWhiteSpace(continuationToken))
                {
                    requestUrl = collectionUrl;
                }
                else
                {
                    var tokenData = _continuationTokenService.Unprotect(continuationToken);
                    ValidateTokenContext(
                        tokenData,
                        resourceKey,
                        protocol,
                        normalizedQuery,
                        requestedPageSize);
                    effectivePageSize = tokenData.PageSize;
                    requestUrl = ValidateContinuationUrl(tokenData.NextLink, collectionUrl).AbsoluteUri;
                }

                var accessToken = await GetAccessToken();
                var client = CreateBusinessCentralClient(accessToken);

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.TryAddWithoutValidation(
                    BusinessCentralDefaults.PreferHeaderName,
                    string.Format(
                        BusinessCentralDefaults.ODataMaxPageSizePreference,
                        effectivePageSize));

                using var response = await client.SendAsync(request);
                await EnsureBusinessCentralSuccessAsync(response, operation);

                var json = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(json);
                if (!document.RootElement.TryGetProperty(
                        BusinessCentralDefaults.ODataCollectionPropertyName,
                        out var valueElement) ||
                    valueElement.ValueKind != JsonValueKind.Array)
                {
                    throw new InternalException(
                        $"Business Central returned a non-collection response for {operation}.");
                }

                var bcPage = JsonSerializer.Deserialize<BcODataResponse<T>>(
                    json,
                    DeserializationOptions)
                    ?? throw new InternalException(
                        $"Business Central returned an invalid collection response for {operation}.");

                if (bcPage.Value == null)
                {
                    throw new InternalException(
                        $"Business Central returned an invalid collection value for {operation}.");
                }

                string? nextToken = null;
                if (!string.IsNullOrWhiteSpace(bcPage.NextLink))
                {
                    var validatedNextLink = ValidateContinuationUrl(
                        bcPage.NextLink,
                        collectionUrl);

                    nextToken = _continuationTokenService.Protect(
                        new BusinessCentralContinuationToken(
                            validatedNextLink.AbsoluteUri,
                            _companyContext.CompanyId,
                            _companyContext.CompanyName,
                            resourceKey,
                            (int)(protocol ?? BcWebServiceProtocol.V2),
                            normalizedQuery,
                            effectivePageSize));
                }

                return new PagedResult<T>
                {
                    Items = bcPage.Value,
                    PageSize = effectivePageSize,
                    HasNext = nextToken != null,
                    NextToken = nextToken
                };
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception ex) when (ex is not InternalException)
            {
                throw new InternalException($"Failed to get paged response: {ex.Message}", ex);
            }
        }

        private void ValidateTokenContext(
            BusinessCentralContinuationToken token,
            string resourceKey,
            BcWebServiceProtocol? protocol,
            string queryString,
            int? requestedPageSize)
        {
            if (token.CompanyId != _companyContext.CompanyId ||
                !string.Equals(
                    token.CompanyName,
                    _companyContext.CompanyName,
                    StringComparison.Ordinal))
            {
                throw new BadRequestException(
                    "The continuation token belongs to a different Business Central company.");
            }

            if (!string.Equals(token.ApiPath, resourceKey, StringComparison.Ordinal) ||
                token.Protocol != (int)(protocol ?? BcWebServiceProtocol.V2))
            {
                throw new BadRequestException(
                    "The continuation token does not belong to this Business Central collection.");
            }

            if (!string.Equals(token.QueryString, queryString, StringComparison.Ordinal))
            {
                throw new BadRequestException(
                    "Search, filters, or sorting changed. Start a new paging sequence without a continuation token.");
            }

            if (requestedPageSize.HasValue && requestedPageSize.Value != token.PageSize)
            {
                throw new BadRequestException(
                    "pageSize cannot change while using a continuation token.");
            }

            ValidatePageSize(token.PageSize);
        }

        private Uri ValidateContinuationUrl(string nextLink, string collectionUrl)
        {
            if (!Uri.TryCreate(collectionUrl, UriKind.Absolute, out var collectionUri))
                throw new InternalException("The configured Business Central collection URL is invalid.");

            if (!Uri.TryCreate(nextLink, UriKind.Absolute, out var continuationUri))
            {
                if (!Uri.TryCreate(collectionUri, nextLink, out continuationUri))
                    throw new BadRequestException("The Business Central continuation URL is invalid.");
            }

            if (!Uri.TryCreate(GetBaseUrl(), UriKind.Absolute, out var configuredBaseUri))
                throw new InternalException("The configured Business Central base URL is invalid.");

            if (!string.IsNullOrEmpty(continuationUri.UserInfo) ||
                !string.Equals(continuationUri.Scheme, configuredBaseUri.Scheme, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(continuationUri.Host, configuredBaseUri.Host, StringComparison.OrdinalIgnoreCase) ||
                continuationUri.Port != configuredBaseUri.Port)
            {
                throw new BadRequestException(
                    "The continuation token contains an unauthorized Business Central URL.");
            }

            var environmentPath = configuredBaseUri.AbsolutePath.TrimEnd('/');
            var isWithinEnvironment = continuationUri.AbsolutePath.Equals(
                    environmentPath,
                    StringComparison.OrdinalIgnoreCase) ||
                continuationUri.AbsolutePath.StartsWith(
                    $"{environmentPath}/",
                    StringComparison.OrdinalIgnoreCase);

            if (!isWithinEnvironment ||
                !continuationUri.AbsolutePath.Equals(
                    collectionUri.AbsolutePath,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException(
                    "The continuation token does not belong to the configured Business Central environment and collection.");
            }

            return continuationUri;
        }

        private static int ValidatePageSize(int? requestedPageSize)
        {
            var pageSize = requestedPageSize ?? BusinessCentralDefaults.DefaultPageSize;

            if (pageSize < 1 || pageSize > BusinessCentralDefaults.MaximumPageSize)
            {
                throw new BadRequestException(
                    $"pageSize must be between 1 and {BusinessCentralDefaults.MaximumPageSize}.");
            }

            return pageSize;
        }

        private static string NormalizeQueryString(string? queryString) =>
            string.IsNullOrWhiteSpace(queryString)
                ? string.Empty
                : queryString.Trim().TrimStart('?', '&');

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
            var baseUrl = GetBaseUrl();
            var protocolPath = GetProtocolPath(bcWebServiceProtocol);

            return $"{baseUrl}/{protocolPath}{apiPath}";
        }

        private string GetProtocolPath(BcWebServiceProtocol? bcWebServiceProtocol)
        {
            return bcWebServiceProtocol switch
            {
                BcWebServiceProtocol.V2 => BusinessCentralDefaults.ProtocolPaths.V2,
                BcWebServiceProtocol.ODataV4 => string.Format(
                    BusinessCentralDefaults.ProtocolPaths.ODataV4,
                    EncodeODataPathCompanyName(_companyContext.CompanyName)),
                BcWebServiceProtocol.V1 => string.Format(
                    BusinessCentralDefaults.ProtocolPaths.V1,
                    _companyContext.CompanyId),
                BcWebServiceProtocol.ItemMasterV1 or BcWebServiceProtocol.CustomerMasterV1 =>
                    string.Format(
                        BusinessCentralDefaults.ProtocolPaths.MefriendV1,
                        _companyContext.CompanyId),
                _ => throw new ArgumentOutOfRangeException(nameof(bcWebServiceProtocol))
            };
        }

        private string BuildNamedODataServiceUrl(string serviceName, string? queryString = null)
        {
            var baseUrl = GetBaseUrl();
            var encodedCompanyName = Uri.EscapeDataString(_companyContext.CompanyName);
            var url = $"{baseUrl}/ODataV4/{serviceName}?company={encodedCompanyName}";

            return AppendQueryString(url, queryString);
        }

        private string GetBaseUrl()
        {
            var baseUrl = _configuration[BusinessCentralDefaults.ConfigurationKeys.BaseUrl];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InternalException(
                    $"{BusinessCentralDefaults.ConfigurationKeys.BaseUrl} configuration value is missing.");
            }

            return baseUrl.TrimEnd('/');
        }

        private static string EncodeODataPathCompanyName(string companyName)
        {
            var escapedODataLiteral = companyName.Replace("'", "''", StringComparison.Ordinal);
            return Uri.EscapeDataString(escapedODataLiteral);
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
