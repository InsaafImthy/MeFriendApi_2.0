using MeFriendApi.Domain.Dto;
using MeFriendApi.Services.Infrastructure;
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
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<ItemMaster>(
                    BusinessCentralDefaults.ApiPaths.ItemMasters,
                    BusinessCentralDefaults.Queries.None,
                    BcWebServiceProtocol.ItemMasterV1),
                "Error retrieving item masters");
        }

        public async Task<IEnumerable<ItemMasterLookupDto>> GetItemMasterLookupsAsync()
        {
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<ItemMasterLookupDto>(
                    BusinessCentralDefaults.ApiPaths.ItemMasters,
                    BusinessCentralDefaults.Queries.ItemMasterLookup,
                    BcWebServiceProtocol.ItemMasterV1),
                "Error retrieving item master lookups");
        }
    }
}
