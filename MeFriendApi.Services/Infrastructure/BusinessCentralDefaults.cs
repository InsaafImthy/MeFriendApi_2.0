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

        internal static class ConfigurationKeys
        {
            internal const string ClientId = "AzureAd:ClientId";
            internal const string ClientSecret = "AzureAd:ClientSecret";
            internal const string TenantId = "AzureAd:TenantId";
            internal const string BaseUrl = "AzureAd:BaseUrl";
            internal const string CompanyId = "CompanyInfo:CompanyId";
            internal const string CompanyName = "CompanyInfo:CompanyName";

            internal static string ApiServiceUrl(string serviceName) =>
                $"BusinessCentralApiServices:{serviceName}:Url";

            internal static string ODataServiceUrl(string serviceName) =>
                $"BusinessCentralODataServices:{serviceName}:Url";
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
        }

        internal static class Queries
        {
            internal const string None = "";
            internal const string CustomerLookup = "?$select=number,name";
            internal const string DimensionsWithValues = "?$filter=code%20eq%20%27PRODUCT%27&$expand=dimensionvalues";
            internal const string ItemMasterLookup = "?$select=number,description,unitPrice";
            internal const string SalesInvoicesWithLines = "?$expand=SalesInvoiceLines";
            internal const string SalespersonLookup = "?$select=code,name";
        }

        internal static class ApiServiceNames
        {
            internal const string Dimensions = "Dimensions";
            internal const string SalesInvoices = "SalesInvoices";
            internal const string Salespersons = "Salespersons";
        }
    }
}
