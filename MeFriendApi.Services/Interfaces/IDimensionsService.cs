using MeFriendApi.Domain.Dto.Dimensions;

namespace MeFriendApi.Services.Interfaces
{
    public interface IDimensionsService
    {
        Task<IEnumerable<DimensionDto>> GetDimensionsAsync();
    }
}
