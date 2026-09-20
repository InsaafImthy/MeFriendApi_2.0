using MeFriendApi.Domain.Dto.SalesOrders;
using MeFriendApi.Services.Infrastructure;
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
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<SalesOrderDto>(
                    BusinessCentralDefaults.ApiPaths.SalesOrders,
                    BusinessCentralDefaults.Queries.None,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error retrieving sales orders");
        }

        public async Task<SalesOrderPostResponse?> CreateSalesOrderAsync(CreateSalesOrderRequest request)
        {
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.PostDataToBc<CreateSalesOrderRequest, SalesOrderPostResponse>(
                    BusinessCentralDefaults.ApiPaths.SalesOrders,
                    request,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error creating sales order");
        }
    }
}
