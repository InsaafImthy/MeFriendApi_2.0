namespace MeFriendApi.Domain.Dto.Paging;

public sealed class PagedRequest
{
    public int? PageSize { get; init; }
    public string? ContinuationToken { get; init; }
    public string? Search { get; init; }
    public string? SortField { get; init; }
    public string? SortDirection { get; init; }
    public Dictionary<string, string?> Filters { get; init; } =
        new(StringComparer.OrdinalIgnoreCase);
}
