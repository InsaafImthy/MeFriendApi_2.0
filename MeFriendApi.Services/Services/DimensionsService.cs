using MeFriendApi.Domain.Dto.Dimensions;
using MeFriendApi.Services.Interfaces;
using static MeFriendApi.Domain.Constants;

namespace MeFriendApi.Services.Services
{
    public class DimensionsService : IDimensionsService
    {
        private readonly ID365CommonService _d365CommonService;

        public DimensionsService(ID365CommonService d365CommonService)
        {
            _d365CommonService = d365CommonService;
        }

        public async Task<IEnumerable<DimensionDto>> GetDimensionsAsync()
        {
            try
            {
                return await _d365CommonService.GetDataFromBc<DimensionDto>(
                    "/dimensions",
                    "?$filter=code%20eq%20%27PRODUCT%27&$expand=dimensionvalues",
                    BcWebServiceProtocol.V1);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving dimensions: {ex.Message}", ex);
            }
        }
    }
}
