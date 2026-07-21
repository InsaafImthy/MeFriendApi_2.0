using MeFriendApi.Domain.Dto;

namespace MeFriendApi.Services.Interfaces
{
    public interface IItemMastersService
    {
        Task<IEnumerable<ItemMaster>> GetItemMastersAsync();
        Task<IEnumerable<ItemMasterLookupDto>> GetItemMasterLookupsAsync();
    }
}
