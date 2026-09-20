using MeFriendApi.Domain.Dto.Salespersons;
using MeFriendApi.Services.Infrastructure;
using MeFriendApi.Services.Interfaces;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class SalespersonsService : ISalespersonsService
    {
        private readonly ID365CommonService _d365CommonService;

        public SalespersonsService(ID365CommonService d365CommonService)
        {
            _d365CommonService = d365CommonService;
        }

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<SalespersonDto>> GetSalespersonsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.Salespersons(request);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<SalespersonDto>(
                    BusinessCentralDefaults.ApiPaths.Salespersons,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving salespersons");
        }

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<SalespersonLookupDto>> GetSalespersonLookupsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.Salespersons(request, lookup: true);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<SalespersonLookupDto>(
                    BusinessCentralDefaults.ApiPaths.Salespersons,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving salesperson lookups");
        }

        public async Task<SalespersonDto?> GetSalespersonAsync(string id)
        {
            var query = ODataQueryBuilder.BuildSingleFilter("code", id);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetSingleDataFromBc<SalespersonDto>(
                    BusinessCentralDefaults.ApiPaths.Salespersons,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving salesperson");
        }
    }
}
