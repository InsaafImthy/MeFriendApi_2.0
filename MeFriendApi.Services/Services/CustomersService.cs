using MeFriendApi.Domain.Dto;
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
            try
            {
                var customers = await _d365CommonService.GetDataFromBc<Customers>(
                    "/customerMasters",
                    "",
                    BcWebServiceProtocol.CustomerMasterV1);
                return customers;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving customers: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CustomerLookupDto>> GetCustomerLookupsAsync()
        {
            try
            {
                return await _d365CommonService.GetDataFromBc<CustomerLookupDto>(
                    "/customerMasters",
                    "?$select=number,name",
                    BcWebServiceProtocol.CustomerMasterV1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving customer lookups: {ex.Message}", ex);
            }
        }

        public async Task<Customers?> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                return await _d365CommonService.PostDataToBc<CreateCustomerRequest, Customers>(
                    "/customerMasters",
                    request,
                    BcWebServiceProtocol.CustomerMasterV1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating customer: {ex.Message}", ex);
            }
        }
    }
}
