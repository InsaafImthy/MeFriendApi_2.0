using MeFriendApi.Domain.Dto.Paging;

namespace MeFriendApi.Services.Infrastructure;

internal static class BusinessCentralListQueries
{
    internal static string Customers(PagedRequest request, bool lookup = false) =>
        ODataQueryBuilder.Build(
            request,
            lookup
                ? ["number", "name"]
                : [
                    "id", "number", "name", "name2", "city", "stateCode",
                    "countryRegionCode", "phoneNumber", "gstRegistrationNo",
                    "locationCode", "gstCustomerType", "createdDateTime", "modifiedDateTime"
                ],
            ["number", "name", "phoneNumber"],
            Map(
                ("number", "number"),
                ("customerName", "name"),
                ("name", "name"),
                ("city", "city"),
                ("stateCode", "stateCode"),
                ("countryCode", "countryRegionCode"),
                ("phoneNumber", "phoneNumber"),
                ("gstCustomerType", "gstCustomerType"),
                ("createdDateTime", "createdDateTime"),
                ("modifiedDateTime", "modifiedDateTime")),
            FilterMap(
                ("city", "city"),
                ("stateCode", "stateCode"),
                ("countryRegionCode", "countryRegionCode"),
                ("locationCode", "locationCode"),
                ("gstCustomerType", "gstCustomerType"),
                ("customerPostingGroup", "customerPostingGroup")),
            "number");

    internal static string SalesOrders(PagedRequest request) =>
        ODataQueryBuilder.Build(
            request,
            null,
            ["number", "sellToCustomerNo", "billToCustomerNo", "roNo"],
            Map(
                ("salesOrderNumber", "number"),
                ("customerCode", "sellToCustomerNo"),
                ("orderDate", "rodate"),
                ("postingDate", "postingDate"),
                ("status", "status"),
                ("salespersonCode", "salesperson"),
                ("invoiceDiscountAmountExclVat", "invoiceDiscountAmountExclVat")),
            FilterMapWithMetadata(
                ("customerCode", "sellToCustomerNo", ODataFilterValueKind.String, ODataFilterOperator.Eq),
                ("status", "status", ODataFilterValueKind.String, ODataFilterOperator.Eq),
                ("salespersonCode", "salesperson", ODataFilterValueKind.String, ODataFilterOperator.Eq),
                ("orderDateFrom", "rodate", ODataFilterValueKind.String, ODataFilterOperator.Ge),
                ("orderDateTo", "rodate", ODataFilterValueKind.String, ODataFilterOperator.Le)));

    internal static string SalesInvoices(PagedRequest request) =>
        ODataQueryBuilder.Build(
            request,
            null,
            ["invoiceNo", "customerCode", "customerName", "clientCode", "clientName", "salesPerson"],
            Map(
                ("invoiceNumber", "invoiceNo"),
                ("customerCode", "customerCode"),
                ("customerName", "customerName"),
                ("clientCode", "clientCode"),
                ("clientName", "clientName"),
                ("invoiceDate", "invoiceDate"),
                ("salespersonCode", "salesPerson"),
                ("netAmount", "netAmount"),
                ("tradeDiscount", "tradeDiscount")),
            FilterMapWithMetadata(
                ("customerCode", "customerCode", ODataFilterValueKind.String, ODataFilterOperator.Eq),
                ("salespersonCode", "salesPerson", ODataFilterValueKind.String, ODataFilterOperator.Eq),
                ("invoiceDateFrom", "invoiceDate", ODataFilterValueKind.Date, ODataFilterOperator.Ge),
                ("invoiceDateTo", "invoiceDate", ODataFilterValueKind.Date, ODataFilterOperator.Le)));

    internal static string Salespersons(PagedRequest request, bool lookup = false) =>
        ODataQueryBuilder.Build(
            request,
            lookup ? ["code", "name"] : ["code", "name", "email", "phone"],
            ["code", "name", "email", "phone"],
            Map(
                ("code", "code"),
                ("salespersonCode", "code"),
                ("name", "name"),
                ("salespersonName", "name"),
                ("email", "email"),
                ("phoneNumber", "phone")),
            FilterMap(("code", "code")),
            "code");

    internal static string ItemMasters(PagedRequest request, bool lookup = false) =>
        ODataQueryBuilder.Build(
            request,
            lookup
                ? ["number", "description", "unitPrice"]
                : null,
            ["number", "description"],
            Map(
                ("number", "number"),
                ("description", "description"),
                ("unitPrice", "unitPrice")),
            null,
            "number");

    internal static string Dimensions(PagedRequest request) =>
        ODataQueryBuilder.Build(
            request,
            ["code", "name"],
            ["code", "name"],
            Map(("code", "code"), ("name", "name")),
            FilterMap(("code", "code")),
            "code");

    internal static string Events(PagedRequest request) =>
        ODataQueryBuilder.Build(
            request,
            null,
            ["code", "name"],
            Map(
                ("code", "code"),
                ("eventCode", "code"),
                ("name", "name"),
                ("eventName", "name")),
            FilterMap(("code", "code")),
            "code");

    private static IReadOnlyDictionary<string, string> Map(
        params (string PublicName, string BusinessCentralName)[] entries) =>
        entries.ToDictionary(
            entry => entry.PublicName,
            entry => entry.BusinessCentralName,
            StringComparer.OrdinalIgnoreCase);

    private static IReadOnlyDictionary<string, ODataFilterField> FilterMap(
        params (string PublicName, string BusinessCentralName)[] entries) =>
        entries.ToDictionary(
            entry => entry.PublicName,
            entry => new ODataFilterField(entry.BusinessCentralName),
            StringComparer.OrdinalIgnoreCase);

    private static IReadOnlyDictionary<string, ODataFilterField> FilterMapWithTypes(
        params (string PublicName, string BusinessCentralName, ODataFilterValueKind ValueKind)[] entries) =>
        entries.ToDictionary(
            entry => entry.PublicName,
            entry => new ODataFilterField(
                entry.BusinessCentralName,
                entry.ValueKind),
            StringComparer.OrdinalIgnoreCase);

    private static IReadOnlyDictionary<string, ODataFilterField> FilterMapWithMetadata(
        params (
            string PublicName,
            string BusinessCentralName,
            ODataFilterValueKind ValueKind,
            ODataFilterOperator Operator)[] entries) =>
        entries.ToDictionary(
            entry => entry.PublicName,
            entry => new ODataFilterField(
                entry.BusinessCentralName,
                entry.ValueKind,
                entry.Operator),
            StringComparer.OrdinalIgnoreCase);
}
