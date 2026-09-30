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
                ("customerCode", "number"),
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
            searchFields: ["number", "sellToCustomerNo", "billToCustomerNo", "roNo"],
            sortFields: Map(
                ("salesOrderNumber", "number"),
                ("customerName", "sellToCustomerName"),
                ("clientName", "clientName"),
                ("orderDate", "rodate"),
                ("postingDate", "postingDate"),
                ("status", "status"),
                ("totalAmount", "amount"),
                ("invoiceDiscountAmountExclVat", "invoiceDiscountAmountExclVat"),
                ("amountIncludingVAT", "amountIncludingVAT")),
            filterFields: FilterMapWithTypes(
                ("customerCode", "sellToCustomerNo", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("salespersonCode", "salesperson", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("status", "status", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("orderDateFrom", "rodate", ODataFilterValueKind.Date, ODataFilterOperator.GreaterThanOrEqual),
                ("orderDateTo", "rodate", ODataFilterValueKind.Date, ODataFilterOperator.LessThanOrEqual)));

    internal static string SalesInvoices(PagedRequest request) =>
        ODataQueryBuilder.Build(
            request,
            searchFields: [
                "invoiceNo", "sellToCustomerNo", "sellToCustomerName",
                "salesOrderNo", "paymentStatus", "status"
            ],
            sortFields: Map(
                ("invoiceNumber", "invoiceNo"),
                ("customerName", "sellToCustomerName"),
                ("salesOrderNumber", "salesOrderNo"),
                ("invoiceDate", "invoiceDate"),
                ("dueDate", "dueDate"),
                ("totalAmount", "totalAmount"),
                ("paidAmount", "paidAmount"),
                ("outstandingAmount", "outstandingAmount"),
                ("paymentStatus", "paymentStatus"),
                ("invoiceStatus", "status")),
            filterFields: FilterMapWithTypes(
                ("customerCode", "sellToCustomerNo", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("salesOrderNumber", "salesOrderNo", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("invoiceStatus", "status", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                ("paymentStatus", "paymentStatus", ODataFilterValueKind.String, ODataFilterOperator.Equal),
                (
                    "invoiceDateFrom",
                    "invoiceDate",
                    ODataFilterValueKind.Date,
                    ODataFilterOperator.GreaterThanOrEqual),
                (
                    "invoiceDateTo",
                    "invoiceDate",
                    ODataFilterValueKind.Date,
                    ODataFilterOperator.LessThanOrEqual),
                ("dueDateFrom", "dueDate", ODataFilterValueKind.Date, ODataFilterOperator.GreaterThanOrEqual),
                ("dueDateTo", "dueDate", ODataFilterValueKind.Date, ODataFilterOperator.LessThanOrEqual),
                ("outstandingOnly", "outstandingAmount", ODataFilterValueKind.PositiveDecimalWhenTrue, ODataFilterOperator.GreaterThan)));

    internal static string Salespersons(PagedRequest request, bool lookup = false) =>
        ODataQueryBuilder.Build(
            request,
            lookup ? ["code", "name"] : ["code", "name", "email", "phone"],
            lookup ? ["code", "name"] : ["code", "name", "email", "phone"],
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
