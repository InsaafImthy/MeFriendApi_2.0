using MeFriendApi.Domain.Dto;

namespace MeFriendApi.Services.Interfaces
{
    public interface ICustomersService
    {
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<Customers>> GetCustomers(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<CustomerLookupDto>> GetCustomerLookupsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<Customers?> GetCustomerAsync(string id);
        Task<Customers?> CreateCustomerAsync(CreateCustomerRequest request);
    }
}
