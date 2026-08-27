using MeFriendApi.Domain.Dto.Salespersons;
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
            try
            {
                return await _d365CommonService.GetDataFromBc<SalespersonDto>(
                    "/salespersons",
                    "",
                    BcWebServiceProtocol.V1,
                    "Salespersons");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving salespersons: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SalespersonLookupDto>> GetSalespersonLookupsAsync()
        {
            try
            {
                return await _d365CommonService.GetDataFromBc<SalespersonLookupDto>(
                    "/salespersons",
                    "?$select=code,name",
                    BcWebServiceProtocol.V1,
                    "Salespersons");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving salesperson lookups: {ex.Message}", ex);
            }
        }
    }
}
