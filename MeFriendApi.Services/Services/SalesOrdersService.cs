using MeFriendApi.Domain.Dto.SalesOrders;
using MeFriendApi.Services.Interfaces;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class SalesOrdersService : ISalesOrdersService
    {
        private readonly ID365CommonService _d365CommonService;

        public SalesOrdersService(ID365CommonService d365CommonService)
        {
            _d365CommonService = d365CommonService;
        }

        public async Task<IEnumerable<SalesOrderDto>> GetSalesOrdersAsync()
        {
            try
            {
                return await _d365CommonService.GetDataFromBc<SalesOrderDto>(
                    "/SalesOrderHeaders",
                    "?$expand=SalesOrderLines",
                    BcWebServiceProtocol.V1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales orders: {ex.Message}", ex);
            }
        }

        public async Task<SalesOrderPostResponse?> CreateSalesOrderAsync(CreateSalesOrderRequest request)
        {
            try
            {
                return await _d365CommonService.PostToODataServiceAsync<CreateSalesOrderRequest, SalesOrderPostResponse>(
                    "MefriendLLP_SalesOrder",
                    request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating sales order: {ex.Message}", ex);
            }
        }
    }
}
