using MeFriendApi.Domain.Dto;
using MeFriendApi.Services.Interfaces;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class ItemMastersService : IItemMastersService
    {
        private readonly ID365CommonService _d365CommonService;

        public ItemMastersService(ID365CommonService d365CommonService)
        {
            _d365CommonService = d365CommonService;
        }

        public async Task<IEnumerable<ItemMaster>> GetItemMastersAsync()
        {
            try
            {
                return await _d365CommonService.GetDataFromBc<ItemMaster>(
                    "/itemMasters",
                    "",
                    BcWebServiceProtocol.ItemMasterV1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving item masters: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ItemMasterLookupDto>> GetItemMasterLookupsAsync()
        {
            try
            {
                return await _d365CommonService.GetDataFromBc<ItemMasterLookupDto>(
                    "/itemMasters",
                    "?$select=number,description",
                    BcWebServiceProtocol.ItemMasterV1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving item master lookups: {ex.Message}", ex);
            }
        }
    }
}
