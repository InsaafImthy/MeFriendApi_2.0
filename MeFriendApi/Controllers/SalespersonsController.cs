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
        public Task<IActionResult> GetSalespersons() =>
            ExecuteAsync(_salespersonsService.GetSalespersonsAsync);

        [HttpGet("lookup")]
        public Task<IActionResult> GetSalespersonLookups() =>
            ExecuteAsync(_salespersonsService.GetSalespersonLookupsAsync);
    }
}
