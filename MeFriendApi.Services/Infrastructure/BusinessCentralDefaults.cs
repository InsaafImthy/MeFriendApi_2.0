namespace MeFriendApi.Services.Infrastructure
{
    internal static class BusinessCentralDefaults
    {
        internal const string HttpClientName = "BusinessCentral";
        internal const string AuthorityBaseUrl = "https://login.microsoftonline.com";
        internal const string AccessTokenScope = "https://api.businesscentral.dynamics.com/.default";
        internal const string JsonMediaType = "application/json";
        internal const string BinaryMediaType = "application/octet-stream";
        internal const string IfMatchHeaderName = "If-Match";
        internal const string MatchAnyEtag = "*";
        internal const string FileFormFieldName = "file";
        internal const string ODataCollectionPropertyName = "value";
        internal const string PreferHeaderName = "Prefer";
        internal const string ODataMaxPageSizePreference = "odata.maxpagesize={0}";
        internal const int DefaultPageSize = 20;
        internal const int MaximumPageSize = 100;

        internal static class HeaderNames
        {
            internal const string CompanyId = "X-BC-Company-Id";
            internal const string CompanyName = "X-BC-Company-Name";
        }

        internal static class ConfigurationKeys
        {
            internal const string ClientId = "AzureAd:ClientId";
            internal const string ClientSecret = "AzureAd:ClientSecret";
            internal const string TenantId = "AzureAd:TenantId";
            internal const string BaseUrl = "AzureAd:BaseUrl";
        }

        internal static class ProtocolPaths
        {
            internal const string V2 = "api/v2.0";
            internal const string ODataV4 = "ODataV4/Company('{0}')";
            internal const string V1 = "api/CVT/CVTGroup/v1.0/Companies({0})";
            internal const string MefriendV1 = "api/aufait/MefriendAPI/v1.0/companies({0})";
        }

        internal static class ApiPaths
        {
            internal const string Customers = "/customerMasters";
            internal const string Dimensions = "/dimensions";
            internal const string ItemMasters = "/itemMasters";
            internal const string SalesInvoices = "/SalesInvoiceHeaders";
            internal const string SalesOrders = "/salesOrders";
            internal const string Salespersons = "/salespersons";
            internal const string Attachments = "/Attachments";
            internal const string DimensionValues = "/dimensionvalues";
        }

    }
}
