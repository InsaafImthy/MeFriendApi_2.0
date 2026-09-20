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

        public async Task<IEnumerable<SalespersonDto>> GetSalespersonsAsync()
        {
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<SalespersonDto>(
                    BusinessCentralDefaults.ApiPaths.Salespersons,
                    BusinessCentralDefaults.Queries.None,
                    BcWebServiceProtocol.V1,
                    BusinessCentralDefaults.ApiServiceNames.Salespersons),
                "Error retrieving salespersons");
        }

        public async Task<IEnumerable<SalespersonLookupDto>> GetSalespersonLookupsAsync()
        {
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<SalespersonLookupDto>(
                    BusinessCentralDefaults.ApiPaths.Salespersons,
                    BusinessCentralDefaults.Queries.SalespersonLookup,
                    BcWebServiceProtocol.V1,
                    BusinessCentralDefaults.ApiServiceNames.Salespersons),
                "Error retrieving salesperson lookups");
        }
    }
}
