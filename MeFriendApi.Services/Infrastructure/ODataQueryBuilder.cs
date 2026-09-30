using MeFriendApi.Domain.Dto.Paging;
using MeFriendApi.Domain.Exceptions;
using System.Globalization;

namespace MeFriendApi.Services.Infrastructure;

internal static class ODataQueryBuilder
{
    internal static string Build(
        PagedRequest request,
        IReadOnlyCollection<string>? selectFields = null,
        IReadOnlyCollection<string>? searchFields = null,
        IReadOnlyDictionary<string, string>? sortFields = null,
        IReadOnlyDictionary<string, ODataFilterField>? filterFields = null,
        string? defaultSortField = null,
        string? requiredFilter = null)
    {
        var query = new List<string>();

        if (selectFields is { Count: > 0 })
            query.Add($"$select={string.Join(',', selectFields)}");

        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(requiredFilter))
            filters.Add(requiredFilter);

        if (!string.IsNullOrWhiteSpace(request.Search) && searchFields is { Count: > 0 })
        {
            var escapedSearch = EscapeStringLiteral(request.Search.Trim());
            filters.Add(
                $"({string.Join(" or ", searchFields.Select(field => $"contains({field},'{escapedSearch}')"))})");
        }

        foreach (var filter in request.Filters.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (filterFields == null || !filterFields.TryGetValue(filter.Key, out var bcField))
                throw new BadRequestException($"Filter field '{filter.Key}' is not supported.");

            if (!string.IsNullOrWhiteSpace(filter.Value))
            {
                var expression = FilterValue(bcField, filter.Value.Trim());
                if (!string.IsNullOrWhiteSpace(expression))
                    filters.Add(expression);
            }
        }

        if (filters.Count > 0)
            query.Add($"$filter={Uri.EscapeDataString(string.Join(" and ", filters))}");

        var requestedSortField = string.IsNullOrWhiteSpace(request.SortField)
            ? defaultSortField
            : request.SortField.Trim();

        if (!string.IsNullOrWhiteSpace(requestedSortField))
        {
            if (sortFields == null || !sortFields.TryGetValue(requestedSortField, out var bcSortField))
                throw new BadRequestException($"Sort field '{requestedSortField}' is not supported.");

            var direction = NormalizeSortDirection(request.SortDirection);
            query.Add($"$orderby={bcSortField}%20{direction}");
        }
        else if (!string.IsNullOrWhiteSpace(request.SortDirection))
        {
            throw new BadRequestException("sortDirection requires a supported sortField.");
        }

        return string.Join('&', query);
    }

    internal static string BuildSingleFilter(
        string field,
        string value,
        bool isGuid = false,
        string? select = null,
        string? expand = null,
        string? scopeField = null,
        string? scopeValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BadRequestException("A record identifier is required.");

        var literal = isGuid
            ? ParseGuid(value).ToString("D")
            : $"'{EscapeStringLiteral(value.Trim())}'";

        var filterExpressions = new List<string> { $"{field} eq {literal}" };

        if (!string.IsNullOrWhiteSpace(scopeField) && !string.IsNullOrWhiteSpace(scopeValue))
        {
            filterExpressions.Add(
                $"{scopeField.Trim()} eq '{EscapeStringLiteral(scopeValue.Trim())}'");
        }

        var query = new List<string>
        {
            $"$filter={Uri.EscapeDataString(string.Join(" and ", filterExpressions))}",
            "$top=1"
        };

        if (!string.IsNullOrWhiteSpace(select))
            query.Add($"$select={select}");

        if (!string.IsNullOrWhiteSpace(expand))
            query.Add($"$expand={expand}");

        return string.Join('&', query);
    }

    private static string? FilterValue(ODataFilterField field, string value)
    {
        if (field.ValueKind == ODataFilterValueKind.PositiveDecimalWhenTrue)
        {
            return ParseBooleanValue(value)
                ? $"{field.BusinessCentralName} gt 0"
                : null;
        }

        var literal = field.ValueKind switch
        {
            ODataFilterValueKind.String => $"'{EscapeStringLiteral(value)}'",
            ODataFilterValueKind.Date => ParseDate(value),
            ODataFilterValueKind.Guid => ParseGuid(value).ToString("D"),
            ODataFilterValueKind.Boolean => ParseBoolean(value),
            ODataFilterValueKind.Decimal => ParseDecimal(value),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };

        var filterOperator = field.Operator switch
        {
            ODataFilterOperator.Equal => "eq",
            ODataFilterOperator.GreaterThanOrEqual => "ge",
            ODataFilterOperator.LessThanOrEqual => "le",
            ODataFilterOperator.GreaterThan => "gt",
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };

        return $"{field.BusinessCentralName} {filterOperator} {literal}";
    }

    private static string EscapeStringLiteral(string value) =>
        value.Replace("'", "''", StringComparison.Ordinal);

    private static Guid ParseGuid(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new BadRequestException("The record identifier must be a valid GUID.");

        return guid;
    }

    private static string ParseDate(string value)
    {
        if (!DateOnly.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            throw new BadRequestException("Date filters must use yyyy-MM-dd format.");
        }

        return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static string ParseBoolean(string value)
    {
        return ParseBooleanValue(value) ? "true" : "false";
    }

    private static bool ParseBooleanValue(string value)
    {
        if (!bool.TryParse(value, out var result))
            throw new BadRequestException("Boolean filters must be true or false.");

        return result;
    }

    private static string ParseDecimal(string value)
    {
        if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
            throw new BadRequestException("Numeric filters must use invariant number format.");

        return result.ToString(CultureInfo.InvariantCulture);
    }

    private static string NormalizeSortDirection(string? direction)
    {
        if (string.IsNullOrWhiteSpace(direction))
            return "asc";

        if (direction.Equals("asc", StringComparison.OrdinalIgnoreCase))
            return "asc";

        if (direction.Equals("desc", StringComparison.OrdinalIgnoreCase))
            return "desc";

        throw new BadRequestException("sortDirection must be either 'asc' or 'desc'.");
    }
}

internal sealed record ODataFilterField(
    string BusinessCentralName,
    ODataFilterValueKind ValueKind = ODataFilterValueKind.String,
    ODataFilterOperator Operator = ODataFilterOperator.Equal);

internal enum ODataFilterOperator
{
    Equal,
    GreaterThanOrEqual,
    LessThanOrEqual,
    GreaterThan
}

internal enum ODataFilterValueKind
{
    String,
    Date,
    Guid,
    Boolean,
    Decimal,
    PositiveDecimalWhenTrue
}
