using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesInvoicesController : ControllerBase
    {
        private readonly ISalesInvoicesService _salesInvoicesService;

        public SalesInvoicesController(ISalesInvoicesService salesInvoicesService)
        {
            _salesInvoicesService = salesInvoicesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesInvoices()
        {
            try
            {
                var salesInvoices = await _salesInvoicesService.GetSalesInvoicesAsync();
                return Ok(salesInvoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
