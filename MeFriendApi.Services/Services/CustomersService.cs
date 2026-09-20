using MeFriendApi.Domain.Dto;
using MeFriendApi.Services.Infrastructure;
using MeFriendApi.Services.Interfaces;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class CustomersService : ICustomersService
    {
        private readonly ID365CommonService _d365CommonService;
        public CustomersService(ID365CommonService d365CommonService)
        {
            _d365CommonService = d365CommonService;
        }

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<Customers>> GetCustomers(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.Customers(request);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<Customers>(
                    BusinessCentralDefaults.ApiPaths.Customers,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error retrieving customers");
        }

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<CustomerLookupDto>> GetCustomerLookupsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.Customers(request, lookup: true);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<CustomerLookupDto>(
                    BusinessCentralDefaults.ApiPaths.Customers,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error retrieving customer lookups");
        }

        public async Task<Customers?> GetCustomerAsync(string id)
        {
            var isGuid = Guid.TryParse(id, out _);
            var query = ODataQueryBuilder.BuildSingleFilter(
                isGuid ? "id" : "number",
                id,
                isGuid);

            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetSingleDataFromBc<Customers>(
                    BusinessCentralDefaults.ApiPaths.Customers,
                    query,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error retrieving customer");
        }

        public async Task<Customers?> CreateCustomerAsync(CreateCustomerRequest request)
        {
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.PostDataToBc<CreateCustomerRequest, Customers>(
                    BusinessCentralDefaults.ApiPaths.Customers,
                    request,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error creating customer");
        }
    }
}
