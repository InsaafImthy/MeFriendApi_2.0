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

        public async Task<IEnumerable<Customers>> GetCustomers()
        {
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<Customers>(
                    BusinessCentralDefaults.ApiPaths.Customers,
                    BusinessCentralDefaults.Queries.None,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error retrieving customers");
        }

        public async Task<IEnumerable<CustomerLookupDto>> GetCustomerLookupsAsync()
        {
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<CustomerLookupDto>(
                    BusinessCentralDefaults.ApiPaths.Customers,
                    BusinessCentralDefaults.Queries.CustomerLookup,
                    BcWebServiceProtocol.CustomerMasterV1),
                "Error retrieving customer lookups");
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
