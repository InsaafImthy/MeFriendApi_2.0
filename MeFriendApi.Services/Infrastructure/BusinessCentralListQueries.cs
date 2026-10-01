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
                    "id", "number", "name", "name2", "address", "address2",
                    "stateCode", "countryRegionCode", "city", "postCode", "locationCode",
                    "phoneNumber", "PAN", "gstRegistrationNo", "genPostingGroup",
                    "customerPostingGroup", "gstCustomerType", "createdDateTime", "modifiedDateTime"
                ],
            ["number"],
            Map(
                ("number", "number"),
                ("customerCode", "number"),
                ("customerName", "name"),
                ("name", "name"),
                ("city", "city"),
                ("countryCode", "countryRegionCode"),
                ("phoneNumber", "phoneNumber"),
                ("createdDateTime", "createdDateTime"),
                ("modifiedDateTime", "modifiedDateTime")),
            FilterMap(
                ("city", "city"),
                ("countryRegionCode", "countryRegionCode"),
                ("locationCode", "locationCode"),
                ("gstCustomerType", "gstCustomerType"),
                ("customerPostingGroup", "customerPostingGroup")),
            "number");

    internal static string SalesOrders(PagedRequest request) =>
        ODataQueryBuilder.Build(
            request,
            searchFields: ["number"],
            sortFields: Map(
                ("salesOrderNumber", "number"),
                ("status", "status")),
            filterFields: FilterMapWithTypes(
                ("customerCode", "sellToCustomerNo", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("salespersonCode", "salesperson", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("status", "status", ODataFilterValueKind.String, ODataFilterOperator.Equal)));

    internal static string SalesInvoices(PagedRequest request) =>
        ODataQueryBuilder.Build(
            request,
            searchFields: ["invoiceNo"],
            sortFields: Map(
                ("invoiceNumber", "invoiceNo"),
                ("customerName", "customerName"),
                ("invoiceDate", "invoiceDate")),
            filterFields: FilterMapWithTypes(
                ("customerCode", "customerCode", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("salespersonCode", "salesPerson", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                (
                    "invoiceDateFrom",
                    "invoiceDate",
                    ODataFilterValueKind.Date,
                    ODataFilterOperator.GreaterThanOrEqual),
                (
                    "invoiceDateTo",
                    "invoiceDate",
                    ODataFilterValueKind.Date,
                    ODataFilterOperator.LessThanOrEqual)));

    internal static string Salespersons(PagedRequest request, bool lookup = false) =>
        ODataQueryBuilder.Build(
            request,
            lookup ? ["code", "name"] : ["code", "name", "email", "phone", "mdmCode"],
            ["code"],
            Map(
                ("code", "code"),
                ("salespersonCode", "code"),
                ("name", "name"),
                ("salespersonName", "name"),
                ("email", "email")),
            FilterMap(("code", "code")),
            "code");

    internal static string ItemMasters(PagedRequest request, bool lookup = false) =>
        ODataQueryBuilder.Build(
            request,
            lookup
                ? ["number", "description", "unitPrice"]
                : null,
            ["number"],
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
            ["code"],
            Map(("code", "code"), ("name", "name")),
            FilterMap(("code", "code")),
            "code");

    internal static string Events(PagedRequest request) =>
        ODataQueryBuilder.Build(
            request,
            null,
            ["code"],
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
