using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemMastersController : ApiControllerBase
    {
        private readonly IItemMastersService _itemMastersService;

        public ItemMastersController(
            IItemMastersService itemMastersService,
            ILogger<ApiControllerBase> logger)
            : base(logger)
        {
            _itemMastersService = itemMastersService;
        }

        [HttpGet]
        public Task<IActionResult> GetItemMasters() =>
            ExecuteAsync(_itemMastersService.GetItemMastersAsync);

        [HttpGet("lookup")]
        public Task<IActionResult> GetItemMasterLookups() =>
            ExecuteAsync(_itemMastersService.GetItemMasterLookupsAsync);
    }
}
