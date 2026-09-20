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
        public Task<IActionResult> GetSalesInvoices(
            [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
            ExecuteAsync(() => _salesInvoicesService.GetSalesInvoicesAsync(request));

        [HttpGet("{id}")]
        public Task<IActionResult> GetSalesInvoice(string id) =>
            ExecuteAsync(() => _salesInvoicesService.GetSalesInvoiceAsync(id));
    }
}
