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
                    "locationCode", "createdDateTime", "modifiedDateTime"
                ],
            ["number", "name", "phoneNumber"],
            Map(
                ("number", "number"),
                ("customerName", "name"),
                ("name", "name"),
                ("city", "city"),
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
        ODataQueryBuilder.Build(request);

    internal static string SalesInvoices(PagedRequest request) =>
        ODataQueryBuilder.Build(request);

    internal static string Salespersons(PagedRequest request, bool lookup = false) =>
        ODataQueryBuilder.Build(
            request,
            ["code", "name"],
            ["code", "name"],
            Map(("code", "code"), ("name", "name")),
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
            Map(("code", "code"), ("name", "name")),
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
}
