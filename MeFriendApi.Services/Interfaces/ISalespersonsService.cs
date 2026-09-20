using MeFriendApi.Domain.Dto.Salespersons;

namespace MeFriendApi.Services.Interfaces
{
    public interface ISalespersonsService
    {
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<SalespersonDto>> GetSalespersonsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<SalespersonLookupDto>> GetSalespersonLookupsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<SalespersonDto?> GetSalespersonAsync(string id);
    }
}
