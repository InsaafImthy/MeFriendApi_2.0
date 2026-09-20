using MeFriendApi.Domain.Dto.SalesOrders;
using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrdersController : ApiControllerBase
    {
        private readonly ISalesOrdersService _salesOrdersService;

        public SalesOrdersController(
            ISalesOrdersService salesOrdersService,
            ILogger<ApiControllerBase> logger)
            : base(logger)
        {
            _salesOrdersService = salesOrdersService;
        }

        [HttpGet]
        public Task<IActionResult> GetSalesOrders() =>
            ExecuteAsync(_salesOrdersService.GetSalesOrdersAsync);

        [HttpPost]
        public Task<IActionResult> CreateSalesOrder(CreateSalesOrderRequest request) =>
            ExecuteAsync(() => _salesOrdersService.CreateSalesOrderAsync(request));
    }
}
