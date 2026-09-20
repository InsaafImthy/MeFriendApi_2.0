using MeFriendApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeFriendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class EventsController : ApiControllerBase
{
    private readonly IDimensionsService _dimensionsService;

    public EventsController(
        IDimensionsService dimensionsService,
        ILogger<ApiControllerBase> logger)
        : base(logger)
    {
        _dimensionsService = dimensionsService;
    }

    [HttpGet]
    public Task<IActionResult> GetEvents(
        [FromQuery] MeFriendApi.Domain.Dto.Paging.PagedRequest request) =>
        ExecuteAsync(() => _dimensionsService.GetEventsAsync(request));

    [HttpGet("{id}")]
    public Task<IActionResult> GetEvent(string id) =>
        ExecuteAsync(() => _dimensionsService.GetEventAsync(id));
}
