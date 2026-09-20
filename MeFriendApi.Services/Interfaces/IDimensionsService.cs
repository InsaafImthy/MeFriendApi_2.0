using MeFriendApi.Domain.Dto.Dimensions;

namespace MeFriendApi.Services.Interfaces
{
    public interface IDimensionsService
    {
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<DimensionDto>> GetDimensionsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<DimensionDto?> GetDimensionAsync(string id);
        Task<MeFriendApi.Domain.Dto.Paging.PagedResult<DimensionValueDto>> GetEventsAsync(
            MeFriendApi.Domain.Dto.Paging.PagedRequest request);
        Task<DimensionValueDto?> GetEventAsync(string id);
    }
}
