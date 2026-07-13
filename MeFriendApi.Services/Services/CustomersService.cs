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
                var customers = await _d365CommonService.GetDataFromBc<Customers>("/customers", "",
                BcWebServiceProtocol.V1);
                return customers;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving customers: {ex.Message}", ex);
            }
        }
    }
}
