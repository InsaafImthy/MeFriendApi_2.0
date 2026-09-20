using MeFriendApi.Domain;
using MeFriendApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MeFriendApi.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        private readonly ILogger<ApiControllerBase> _logger;

        protected ApiControllerBase(ILogger<ApiControllerBase> logger)
        {
            _logger = logger;
        }

        protected async Task<IActionResult> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            try
            {
                return Ok(await operation());
            }
            catch (BadRequestException ex)
            {
                LogActionFailure(ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                LogActionFailure(ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"{Messages.InternalServerErrorPrefix}{ex.Message}");
            }
        }

        protected async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> operation)
        {
            try
            {
                return await operation();
            }
            catch (BadRequestException ex)
            {
                LogActionFailure(ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                LogActionFailure(ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"{Messages.InternalServerErrorPrefix}{ex.Message}");
            }
        }

        private void LogActionFailure(Exception exception)
        {
            _logger.LogError(
                exception,
                "API request processing failed in {ControllerName}.",
                GetType().Name);
        }
    }
}
