using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesInvoicesController : ApiControllerBase
    {
        private readonly ISalesInvoicesService _salesInvoicesService;

        public SalesInvoicesController(
            ISalesInvoicesService salesInvoicesService,
            ILogger<ApiControllerBase> logger)
            : base(logger)
        {
            _salesInvoicesService = salesInvoicesService;
        }

        [HttpGet]
        public Task<IActionResult> GetSalesInvoices() =>
            ExecuteAsync(_salesInvoicesService.GetSalesInvoicesAsync);
    }
}
