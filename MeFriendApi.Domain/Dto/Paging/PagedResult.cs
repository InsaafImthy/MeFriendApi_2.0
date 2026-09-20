namespace MeFriendApi.Domain.Dto.Paging;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int PageSize { get; init; }
    public bool HasNext { get; init; }
    public string? NextToken { get; init; }
}
