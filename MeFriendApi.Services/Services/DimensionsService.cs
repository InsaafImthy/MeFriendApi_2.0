using MeFriendApi.Domain.Dto.Dimensions;
using MeFriendApi.Services.Infrastructure;
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
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetDataFromBc<DimensionDto>(
                    BusinessCentralDefaults.ApiPaths.Dimensions,
                    BusinessCentralDefaults.Queries.DimensionsWithValues,
                    BcWebServiceProtocol.V1,
                    BusinessCentralDefaults.ApiServiceNames.Dimensions),
                "Error retrieving dimensions");
        }
    }
}
