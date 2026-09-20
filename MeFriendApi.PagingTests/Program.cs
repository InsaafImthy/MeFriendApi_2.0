using MeFriendApi.Domain.Dto;
using MeFriendApi.Domain.Dto.Paging;
using MeFriendApi.Domain.Exceptions;
using MeFriendApi.Services.Infrastructure;
using MeFriendApi.Services.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using static MeFriendApi.Domain.Constants;

var tests = new PagingTests();
await tests.RunAsync();

internal sealed class PagingTests
{
    private static readonly Guid CompanyA = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid CompanyB = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private const string BaseUrl =
        "https://api.businesscentral.dynamics.com/v2.0/test-tenant/Sandbox";
    private readonly BusinessCentralContinuationTokenService _tokens =
        new(new EphemeralDataProtectionProvider());

    public async Task RunAsync()
    {
        await FirstAndNextPageAsync();
        await InvalidTokenIsRejectedAsync();
        await CompanyTokenIsolationAsync();
        await ContinuationUrlCannotEscapeEnvironmentAsync();
        await MaximumPageSizeIsEnforcedAsync();
        await CustomerFiltersAndSortingStayServerSideAsync();
        await CollectionEndpointsUseExactCustomSchemasAsync();
        await InvoiceListAndDetailAreSeparatedAsync();
        await EventsUsePagedNavigationCollectionAsync();

        Console.WriteLine("PASS: 9 Business Central paging test groups completed.");
    }

    private async Task FirstAndNextPageAsync()
    {
        var collectionPath = ApiPath(CompanyA, "/customerMasters");
        var nextLink = $"{BaseUrl}{collectionPath}?$skiptoken=next-page";
        var firstItems = string.Join(
            ',',
            Enumerable.Range(1, 20).Select(index => $"{{\"number\":\"C{index}\"}}"));
        var handler = new QueueHandler(
            Json($"{{\"value\":[{firstItems}],\"@odata.nextLink\":\"{nextLink}\"}}"),
            Json("{\"value\":[{\"number\":\"C21\"}]}"));
        var service = CreateService(handler, CompanyA);

        var first = await service.GetPagedDataFromBc<Customers>(
            "/customerMasters",
            20,
            queryString: "$select=number",
            bcWebServiceProtocol: BcWebServiceProtocol.CustomerMasterV1);

        Assert(first.Items.Count == 20 && first.Items[0].Number == "C1", "first page maximum items");
        Assert(first.HasNext && !string.IsNullOrWhiteSpace(first.NextToken), "next token created");
        Assert(first.PageSize == 20, "page size returned");
        Assert(handler.Requests[0].Prefer == "odata.maxpagesize=20", "Prefer header");

        var second = await service.GetPagedDataFromBc<Customers>(
            "/customerMasters",
            20,
            first.NextToken,
            "$select=number",
            BcWebServiceProtocol.CustomerMasterV1);

        Assert(second.Items.Count == 1 && second.Items[0].Number == "C21", "second page items");
        Assert(!second.HasNext && second.NextToken == null, "last page has no token");
        Assert(handler.Requests[1].Uri.Query.Contains("$skiptoken=next-page"), "nextLink executed");
    }

    private async Task InvalidTokenIsRejectedAsync()
    {
        var service = CreateService(new QueueHandler(), CompanyA);
        await AssertThrowsAsync<BadRequestException>(
            () => service.GetPagedDataFromBc<Customers>(
                "/customerMasters",
                20,
                "not-a-valid-token",
                "$select=number",
                BcWebServiceProtocol.CustomerMasterV1),
            "invalid token");
    }

    private async Task CompanyTokenIsolationAsync()
    {
        var path = ApiPath(CompanyA, "/customerMasters");
        var handlerA = new QueueHandler(
            Json($"{{\"value\":[],\"@odata.nextLink\":\"{BaseUrl}{path}?$skiptoken=a\"}}"));
        var serviceA = CreateService(handlerA, CompanyA);
        var first = await serviceA.GetPagedDataFromBc<Customers>(
            "/customerMasters",
            queryString: "$select=number",
            bcWebServiceProtocol: BcWebServiceProtocol.CustomerMasterV1);

        var serviceB = CreateService(new QueueHandler(), CompanyB);
        await AssertThrowsAsync<BadRequestException>(
            () => serviceB.GetPagedDataFromBc<Customers>(
                "/customerMasters",
                continuationToken: first.NextToken,
                queryString: "$select=number",
                bcWebServiceProtocol: BcWebServiceProtocol.CustomerMasterV1),
            "company token isolation");
    }

    private async Task MaximumPageSizeIsEnforcedAsync()
    {
        var service = CreateService(new QueueHandler(), CompanyA);
        await AssertThrowsAsync<BadRequestException>(
            () => service.GetPagedDataFromBc<Customers>(
                "/customerMasters",
                101,
                bcWebServiceProtocol: BcWebServiceProtocol.CustomerMasterV1),
            "maximum page size");
    }

    private async Task ContinuationUrlCannotEscapeEnvironmentAsync()
    {
        const string query = "$select=number";
        var maliciousToken = _tokens.Protect(
            new BusinessCentralContinuationToken(
                "https://example.invalid/steal",
                CompanyA,
                "CRONUS IN",
                "/customerMasters",
                (int)BcWebServiceProtocol.CustomerMasterV1,
                query,
                20));
        var handler = new QueueHandler();
        var service = CreateService(handler, CompanyA);

        await AssertThrowsAsync<BadRequestException>(
            () => service.GetPagedDataFromBc<Customers>(
                "/customerMasters",
                20,
                maliciousToken,
                query,
                BcWebServiceProtocol.CustomerMasterV1),
            "continuation SSRF protection");
        Assert(handler.Requests.Count == 0, "unauthorized continuation URL was not requested");
    }

    private async Task CustomerFiltersAndSortingStayServerSideAsync()
    {
        var handler = new QueueHandler(Json("{\"value\":[]}"));
        var common = CreateService(handler, CompanyA);
        var customers = new CustomersService(common);

        await customers.GetCustomers(new PagedRequest
        {
            Search = "O'Reilly",
            SortField = "customerName",
            SortDirection = "desc",
            Filters = new Dictionary<string, string?> { ["city"] = "Mumbai" }
        });

        var query = Uri.UnescapeDataString(handler.Requests.Single().Uri.Query);
        Assert(query.Contains("contains(name,'O''Reilly')"), "escaped server search");
        Assert(query.Contains("city eq 'Mumbai'"), "allowlisted server filter");
        Assert(query.Contains("$orderby=name desc"), "allowlisted server sort");
        Assert(query.Contains("$select=id,number,name"), "customer list select");
    }

    private async Task InvoiceListAndDetailAreSeparatedAsync()
    {
        var handler = new QueueHandler(
            Json("{\"value\":[]}"),
            Json("{\"value\":[{\"invoiceNo\":\"INV-1\",\"SalesInvoiceLines\":[]}]}"));
        var invoices = new SalesInvoicesService(CreateService(handler, CompanyA));

        await invoices.GetSalesInvoicesAsync(new PagedRequest());
        await invoices.GetSalesInvoiceAsync("INV-1");

        var listQuery = Uri.UnescapeDataString(handler.Requests[0].Uri.Query);
        var detailQuery = Uri.UnescapeDataString(handler.Requests[1].Uri.Query);
        Assert(!listQuery.Contains("$expand", StringComparison.OrdinalIgnoreCase), "invoice list has no expand");
        Assert(!listQuery.Contains("base64", StringComparison.OrdinalIgnoreCase), "invoice list excludes base64");
        Assert(detailQuery.Contains("$top=1"), "invoice detail is limited to one");
        Assert(detailQuery.Contains("$expand=SalesInvoiceLines"), "invoice detail expands lines");
    }

    private async Task CollectionEndpointsUseExactCustomSchemasAsync()
    {
        var customerHandler = new QueueHandler(Json("{\"value\":[]}"));
        await new CustomersService(CreateService(customerHandler, CompanyA))
            .GetCustomers(new PagedRequest());
        AssertAufaitPath(customerHandler, "/customerMasters", "customers");

        var orderHandler = new QueueHandler(Json("{\"value\":[{\"number\":\"SO-1\"}]}"));
        var orders = await new SalesOrdersService(CreateService(orderHandler, CompanyA))
            .GetSalesOrdersAsync(new PagedRequest());
        AssertAufaitPath(orderHandler, "/salesOrders", "sales orders");
        Assert(orders.Items.Single().No == "SO-1", "sales order number maps to public no property");
        AssertNoQueryOption(orderHandler, "$select", "sales orders select");
        AssertNoQueryOption(orderHandler, "$orderby", "sales orders orderby");
        AssertNoQueryOption(orderHandler, "$filter", "sales orders filter");

        var invoiceHandler = new QueueHandler(Json("{\"value\":[]}"));
        await new SalesInvoicesService(CreateService(invoiceHandler, CompanyA))
            .GetSalesInvoicesAsync(new PagedRequest());
        AssertCvtPath(invoiceHandler, "/SalesInvoiceHeaders", "sales invoices");
        AssertNoQueryOption(invoiceHandler, "$select", "sales invoices select");
        AssertNoQueryOption(invoiceHandler, "$orderby", "sales invoices orderby");
        AssertNoQueryOption(invoiceHandler, "$filter", "sales invoices filter");

        var salespersonHandler = new QueueHandler(Json("{\"value\":[]}"));
        await new SalespersonsService(CreateService(salespersonHandler, CompanyA))
            .GetSalespersonsAsync(new PagedRequest());
        AssertCvtPath(salespersonHandler, "/salespersons", "salespersons");
        var salespersonQuery = DecodedQuery(salespersonHandler);
        Assert(salespersonQuery.Contains("$select=code,name"), "salespersons use proven fields");
        Assert(!salespersonQuery.Contains("phoneNo", StringComparison.OrdinalIgnoreCase), "salespersons exclude phoneNo");
        Assert(!salespersonQuery.Contains("email", StringComparison.OrdinalIgnoreCase), "salespersons exclude unverified email");

        var dimensionHandler = new QueueHandler(Json("{\"value\":[]}"));
        await new DimensionsService(CreateService(dimensionHandler, CompanyA))
            .GetDimensionsAsync(new PagedRequest());
        AssertCvtPath(dimensionHandler, "/dimensions", "dimensions");
        var dimensionQuery = DecodedQuery(dimensionHandler);
        Assert(dimensionQuery.Contains("$select=code,name"), "dimensions use proven fields");
        Assert(!dimensionQuery.Contains("id", StringComparison.OrdinalIgnoreCase), "dimensions exclude id");

        var itemHandler = new QueueHandler(Json("{\"value\":[]}"));
        await new ItemMastersService(CreateService(itemHandler, CompanyA))
            .GetItemMastersAsync(new PagedRequest());
        AssertAufaitPath(itemHandler, "/itemMasters", "items");
        AssertNoQueryOption(itemHandler, "$select", "items select");

        foreach (var request in new[]
                 {
                     customerHandler.Requests.Single(),
                     orderHandler.Requests.Single(),
                     invoiceHandler.Requests.Single(),
                     salespersonHandler.Requests.Single(),
                     dimensionHandler.Requests.Single(),
                     itemHandler.Requests.Single()
                 })
        {
            Assert(request.Prefer == "odata.maxpagesize=20", $"Prefer header for {request.Uri.AbsolutePath}");
        }
    }

    private async Task EventsUsePagedNavigationCollectionAsync()
    {
        var handler = new QueueHandler(
            Json("{\"value\":[{\"code\":\"PRODUCT\",\"name\":\"Product\"}]}"),
            Json("{\"value\":[{\"code\":\"EVENT-1\",\"name\":\"Event 1\"}]}"));
        var dimensions = new DimensionsService(CreateService(handler, CompanyA));

        var result = await dimensions.GetEventsAsync(new PagedRequest { PageSize = 20 });

        Assert(result.Items.Count == 1, "event page returned");
        Assert(handler.Requests.Count == 2, "event uses metadata plus page request");
        Assert(
            handler.Requests[1].Uri.AbsolutePath.EndsWith(
                "/dimensions('PRODUCT')/dimensionvalues",
                StringComparison.OrdinalIgnoreCase),
            "dimension-values navigation endpoint uses the custom code key");
        Assert(
            !DecodedQuery(handler, 1).Contains("$select", StringComparison.OrdinalIgnoreCase),
            "events do not select unverified child fields");
        Assert(
            !DecodedQuery(handler, 1).Contains("$expand", StringComparison.OrdinalIgnoreCase),
            "events do not expand all dimension values");
        Assert(handler.Requests[1].Prefer == "odata.maxpagesize=20", "events retain server paging");
    }

    private static void AssertAufaitPath(QueueHandler handler, string resource, string description) =>
        Assert(
            handler.Requests.Single().Uri.AbsolutePath.EndsWith(
                $"/api/aufait/MefriendAPI/v1.0/companies({CompanyA:D}){resource}",
                StringComparison.OrdinalIgnoreCase),
            $"{description} keep Aufait MefriendAPI v1.0 endpoint");

    private static void AssertCvtPath(QueueHandler handler, string resource, string description) =>
        Assert(
            handler.Requests.Single().Uri.AbsolutePath.EndsWith(
                $"/api/CVT/CVTGroup/v1.0/Companies({CompanyA:D}){resource}",
                StringComparison.OrdinalIgnoreCase),
            $"{description} keep CVTGroup v1.0 endpoint");

    private static void AssertNoQueryOption(QueueHandler handler, string option, string description) =>
        Assert(
            !DecodedQuery(handler).Contains(option, StringComparison.OrdinalIgnoreCase),
            $"{description} omitted");

    private static string DecodedQuery(QueueHandler handler, int index = 0) =>
        Uri.UnescapeDataString(handler.Requests[index].Uri.Query);

    private TestD365CommonService CreateService(QueueHandler handler, Guid companyId)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureAd:BaseUrl"] = BaseUrl,
                ["AzureAd:ClientId"] = "test-client",
                ["AzureAd:ClientSecret"] = "test-secret",
                ["AzureAd:TenantId"] = "test-tenant"
            })
            .Build();

        return new TestD365CommonService(
            configuration,
            new TestHttpClientFactory(handler),
            new TestCompanyContext(companyId, "CRONUS IN"),
            _tokens);
    }

    private static string ApiPath(Guid companyId, string resource) =>
        $"/api/aufait/MefriendAPI/v1.0/companies({companyId:D}){resource}";

    private static HttpResponseMessage Json(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    private static void Assert(bool condition, string description)
    {
        if (!condition)
            throw new InvalidOperationException($"FAIL: {description}");
    }

    private static async Task AssertThrowsAsync<TException>(
        Func<Task> action,
        string description)
        where TException : Exception
    {
        try
        {
            await action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException($"FAIL: {description} did not throw {typeof(TException).Name}");
    }
}

internal sealed class TestD365CommonService : D365CommonService
{
    public TestD365CommonService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IBusinessCentralCompanyContext companyContext,
        IBusinessCentralContinuationTokenService tokenService)
        : base(configuration, httpClientFactory, companyContext, tokenService)
    {
    }

    public override Task<string> GetAccessToken() => Task.FromResult("test-token");
}

internal sealed record TestCompanyContext(Guid CompanyId, string CompanyName)
    : IBusinessCentralCompanyContext;

internal sealed class TestHttpClientFactory : IHttpClientFactory
{
    private readonly HttpMessageHandler _handler;

    public TestHttpClientFactory(HttpMessageHandler handler)
    {
        _handler = handler;
    }

    public HttpClient CreateClient(string name) => new(_handler, disposeHandler: false);
}

internal sealed class QueueHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses;
    public List<CapturedRequest> Requests { get; } = [];

    public QueueHandler(params HttpResponseMessage[] responses)
    {
        _responses = new Queue<HttpResponseMessage>(responses);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Requests.Add(new CapturedRequest(
            request.RequestUri ?? throw new InvalidOperationException("Request URI missing."),
            request.Headers.TryGetValues("Prefer", out var values)
                ? values.Single()
                : null));

        if (_responses.Count == 0)
            throw new InvalidOperationException("An unexpected HTTP request was sent.");

        return Task.FromResult(_responses.Dequeue());
    }
}

internal sealed record CapturedRequest(Uri Uri, string? Prefer);
