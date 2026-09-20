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

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<DimensionDto>> GetDimensionsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var query = BusinessCentralListQueries.Dimensions(request);
            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<DimensionDto>(
                    BusinessCentralDefaults.ApiPaths.Dimensions,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving dimensions");
        }

        public async Task<DimensionDto?> GetDimensionAsync(string id)
        {
            var query = ODataQueryBuilder.BuildSingleFilter(
                "code",
                id);

            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetSingleDataFromBc<DimensionDto>(
                    BusinessCentralDefaults.ApiPaths.Dimensions,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving dimension");
        }

        public async Task<MeFriendApi.Domain.Dto.Paging.PagedResult<DimensionValueDto>> GetEventsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request)
        {
            var productDimension = await GetProductDimensionAsync();
            var path = BuildDimensionValuesPath(productDimension?.Code);
            var query = BusinessCentralListQueries.Events(request);

            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetPagedDataFromBc<DimensionValueDto>(
                    path,
                    request.PageSize,
                    request.ContinuationToken,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving PRODUCT dimension values");
        }

        public async Task<DimensionValueDto?> GetEventAsync(string id)
        {
            var productDimension = await GetProductDimensionAsync();
            var path = BuildDimensionValuesPath(productDimension?.Code);
            var query = ODataQueryBuilder.BuildSingleFilter(
                "code",
                id);

            return await ServiceOperationExecutor.ExecuteAsync(
                () => _d365CommonService.GetSingleDataFromBc<DimensionValueDto>(
                    path,
                    query,
                    BcWebServiceProtocol.V1),
                "Error retrieving PRODUCT dimension value");
        }

        private Task<DimensionDto?> GetProductDimensionAsync()
        {
            var query = ODataQueryBuilder.BuildSingleFilter(
                "code",
                "PRODUCT",
                select: "code,name");

            return _d365CommonService.GetSingleDataFromBc<DimensionDto>(
                BusinessCentralDefaults.ApiPaths.Dimensions,
                query,
                BcWebServiceProtocol.V1);
        }

        private static string BuildDimensionValuesPath(string? dimensionCode)
        {
            if (string.IsNullOrWhiteSpace(dimensionCode))
            {
                throw new MeFriendApi.Domain.Exceptions.InternalException(
                    "The PRODUCT dimension API response does not expose the code required to page dimension values directly.");
            }

            var escapedCode = dimensionCode.Replace("'", "''", StringComparison.Ordinal);
            return $"{BusinessCentralDefaults.ApiPaths.Dimensions}('{escapedCode}')" +
                BusinessCentralDefaults.ApiPaths.DimensionValues;
        }
    }
}
