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

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<SalesOrderDto>> GetSalesOrdersAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.SalesOrders(request);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<SalesOrderDto>(
                    BusinessCentralDefaults.ApiPaths.SalesOrders,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error retrieving sales orders");
        }

        public async Task<SalesOrderDto?> GetSalesOrderAsync(string id)
        {
            var query = ODataQueryBuilder.BuildSingleFilter(
                "number",
                id,
                expand: "SalesOrderLines");

            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetSingleDataFromBc<SalesOrderDto>(
                    BusinessCentralDefaults.ApiPaths.SalesOrders,
                    query,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error retrieving sales order");
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
