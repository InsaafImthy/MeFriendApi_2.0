using MeFriendApi.Domain;
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
        public Task<IActionResult> GetDimensions() =>
            ExecuteAsync(async () =>
            {
                var dimensions = await _dimensionsService.GetDimensionsAsync();
                var productDimension = dimensions.FirstOrDefault();

                if (productDimension == null)
                {
                    return NotFound(Messages.ProductDimensionNotFound);
                }

                return Ok(productDimension);
            });
    }
}
