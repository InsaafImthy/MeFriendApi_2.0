using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalespersonsController : ControllerBase
    {
        private readonly ISalespersonsService _salespersonsService;

        public SalespersonsController(ISalespersonsService salespersonsService)
        {
            _salespersonsService = salespersonsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalespersons()
        {
            try
            {
                var salespersons = await _salespersonsService.GetSalespersonsAsync();
                return Ok(salespersons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
