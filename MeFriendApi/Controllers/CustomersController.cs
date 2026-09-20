using MeFriendApi.Domain.Dto;
using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ApiControllerBase
    {
        private readonly ICustomersService _customersService;
        public CustomersController(
            ICustomersService customersService,
            ILogger<ApiControllerBase> logger)
            : base(logger)
        {
            _customersService = customersService;
        }

        [HttpGet]
        public Task<IActionResult> GetCustomers(
            [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
            ExecuteAsync(() => _customersService.GetCustomers(request));

        [HttpGet("lookup")]
        public Task<IActionResult> GetCustomerLookups(
            [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
            ExecuteAsync(() => _customersService.GetCustomerLookupsAsync(request));

        [HttpGet("{id}")]
        public Task<IActionResult> GetCustomer(string id) =>
            ExecuteAsync(() => _customersService.GetCustomerAsync(id));

        [HttpPost]
        public Task<IActionResult> CreateCustomer(CreateCustomerRequest request) =>
            ExecuteAsync(() => _customersService.CreateCustomerAsync(request));
    }
}
