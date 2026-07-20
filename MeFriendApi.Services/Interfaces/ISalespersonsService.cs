using MeFriendApi.Domain.Dto.Salespersons;

namespace MeFriendApi.Services.Interfaces
{
    public interface ISalespersonsService
    {
        Task<IEnumerable<SalespersonDto>> GetSalespersonsAsync();
        Task<IEnumerable<SalespersonLookupDto>> GetSalespersonLookupsAsync();
    }
}
