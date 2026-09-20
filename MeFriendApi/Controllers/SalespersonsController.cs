using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalespersonsController : ApiControllerBase
    {
        private readonly ISalespersonsService _salespersonsService;

        public SalespersonsController(
            ISalespersonsService salespersonsService,
            ILogger<ApiControllerBase> logger)
            : base(logger)
        {
            _salespersonsService = salespersonsService;
        }

        [HttpGet]
        public Task<IActionResult> GetSalespersons(
            [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
            ExecuteAsync(() => _salespersonsService.GetSalespersonsAsync(request));

        [HttpGet("lookup")]
        public Task<IActionResult> GetSalespersonLookups(
            [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
            ExecuteAsync(() => _salespersonsService.GetSalespersonLookupsAsync(request));

        [HttpGet("{id}")]
        public Task<IActionResult> GetSalesperson(string id) =>
            ExecuteAsync(() => _salespersonsService.GetSalespersonAsync(id));
    }
}
