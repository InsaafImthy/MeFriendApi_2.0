using MeFriendApi.Domain.Dto;

namespace MeFriendApi.Services.Interfaces
{
    public interface IItemMastersService
    {
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<ItemMaster>> GetItemMastersAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<ItemMasterLookupDto>> GetItemMasterLookupsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<ItemMaster?> GetItemMasterAsync(string id);
    }
}
