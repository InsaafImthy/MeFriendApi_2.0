using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DimensionsController : ControllerBase
    {
        private readonly IDimensionsService _dimensionsService;

        public DimensionsController(IDimensionsService dimensionsService)
        {
            _dimensionsService = dimensionsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDimensions()
        {
            try
            {
                var dimensions = await _dimensionsService.GetDimensionsAsync();
                return Ok(dimensions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
