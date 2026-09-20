using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DimensionsController : ApiControllerBase
    {
        private readonly IDimensionsService _dimensionsService;

        public DimensionsController(
            IDimensionsService dimensionsService,
            ILogger<ApiControllerBase> logger)
            : base(logger)
        {
            _dimensionsService = dimensionsService;
        }

        [HttpGet]
        public Task<IActionResult> GetDimensions(
            [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
            ExecuteAsync(() => _dimensionsService.GetDimensionsAsync(request));

        [HttpGet("{id}")]
        public Task<IActionResult> GetDimension(string id) =>
            ExecuteAsync(() => _dimensionsService.GetDimensionAsync(id));
    }
}
