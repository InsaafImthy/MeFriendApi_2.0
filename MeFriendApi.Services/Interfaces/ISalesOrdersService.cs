using MeFriendApi.Domain.Dto.SalesOrders;

namespace MeFriendApi.Services.Interfaces
{
    public interface ISalesOrdersService
    {
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<SalesOrderDto>> GetSalesOrdersAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<SalesOrderDto?> GetSalesOrderAsync(string id);
        Task<SalesOrderPostResponse?> CreateSalesOrderAsync(CreateSalesOrderRequest request);
    }
}
