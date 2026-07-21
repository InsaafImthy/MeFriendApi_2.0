using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemMastersController : ControllerBase
    {
        private readonly IItemMastersService _itemMastersService;

        public ItemMastersController(IItemMastersService itemMastersService)
        {
            _itemMastersService = itemMastersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItemMasters()
        {
            try
            {
                var itemMasters = await _itemMastersService.GetItemMastersAsync();
                return Ok(itemMasters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetItemMasterLookups()
        {
            try
            {
                var itemMasters = await _itemMastersService.GetItemMasterLookupsAsync();
                return Ok(itemMasters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
