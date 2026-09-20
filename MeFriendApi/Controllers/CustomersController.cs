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
        public Task<IActionResult> GetCustomers() =>
            ExecuteAsync(_customersService.GetCustomers);

        [HttpGet("lookup")]
        public Task<IActionResult> GetCustomerLookups() =>
            ExecuteAsync(_customersService.GetCustomerLookupsAsync);

        [HttpPost]
        public Task<IActionResult> CreateCustomer(CreateCustomerRequest request) =>
            ExecuteAsync(() => _customersService.CreateCustomerAsync(request));
    }
}
