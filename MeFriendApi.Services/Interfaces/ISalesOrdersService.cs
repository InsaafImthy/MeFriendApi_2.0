using MeFriendApi.Domain.Dto.SalesOrders;

namespace MeFriendApi.Services.Interfaces
{
    public interface ISalesOrdersService
    {
        Task<IEnumerable<SalesOrderDto>> GetSalesOrdersAsync();
        Task<SalesOrderPostResponse?> CreateSalesOrderAsync(CreateSalesOrderRequest request);
    }
}
