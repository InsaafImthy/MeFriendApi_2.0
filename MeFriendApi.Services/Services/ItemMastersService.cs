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

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<ItemMaster>> GetItemMastersAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.ItemMasters(request);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<ItemMaster>(
                    BusinessCentralDefaults.ApiPaths.ItemMasters,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.ItemMasterV1),
                "Error retrieving item masters");
        }

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<ItemMasterLookupDto>> GetItemMasterLookupsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.ItemMasters(request, lookup: true);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<ItemMasterLookupDto>(
                    BusinessCentralDefaults.ApiPaths.ItemMasters,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.ItemMasterV1),
                "Error retrieving item master lookups");
        }

        public async Task<ItemMaster?> GetItemMasterAsync(string id)
        {
            var query = ODataQueryBuilder.BuildSingleFilter("number", id);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetSingleDataFromBc<ItemMaster>(
                    BusinessCentralDefaults.ApiPaths.ItemMasters,
                    query,
                    BcWebServiceProtocol.ItemMasterV1),
                "Error retrieving item master");
        }
    }
}
