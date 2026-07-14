using MeFriendApi.Domain.Dto.SalesOrders;
using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrdersService _salesOrdersService;

        public SalesOrdersController(ISalesOrdersService salesOrdersService)
        {
            _salesOrdersService = salesOrdersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrders()
        {
            try
            {
                var salesOrders = await _salesOrdersService.GetSalesOrdersAsync();
                return Ok(salesOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalesOrder(CreateSalesOrderRequest request)
        {
            try
            {
                var salesOrder = await _salesOrdersService.CreateSalesOrderAsync(request);
                return Ok(salesOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
