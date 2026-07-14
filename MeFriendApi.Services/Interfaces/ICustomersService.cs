using MeFriendApi.Domain.Dto;

namespace MeFriendApi.Services.Interfaces
{
    public interface ICustomersService
    {
        Task<IEnumerable<Customers>> GetCustomers();
        Task<Customers?> CreateCustomerAsync(CreateCustomerRequest request);
    }
}
