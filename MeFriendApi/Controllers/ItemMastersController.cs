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
        public Task<IActionResult> GetItemMasters(
            [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
            ExecuteAsync(() => _itemMastersService.GetItemMastersAsync(request));

        [HttpGet("lookup")]
        public Task<IActionResult> GetItemMasterLookups(
            [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
            ExecuteAsync(() => _itemMastersService.GetItemMasterLookupsAsync(request));

        [HttpGet("{id}")]
        public Task<IActionResult> GetItemMaster(string id) =>
            ExecuteAsync(() => _itemMastersService.GetItemMasterAsync(id));
    }
}
