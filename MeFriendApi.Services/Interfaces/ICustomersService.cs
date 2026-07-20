using MeFriendApi.Domain.Dto;

namespace MeFriendApi.Services.Interfaces
{
    public interface ICustomersService
    {
        Task<IEnumerable<Customers>> GetCustomers();
        Task<IEnumerable<CustomerLookupDto>> GetCustomerLookupsAsync();
        Task<Customers?> CreateCustomerAsync(CreateCustomerRequest request);
    }
}
