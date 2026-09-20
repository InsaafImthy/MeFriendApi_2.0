using MeFriendApi.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using static MeFriendApi.Domain.Dto.Helpers.CommonDto;

namespace MeFriendApi.Services.Middlewares
{
    public class UserAuthMiddleware : IMiddleware
    {
        private readonly ILogger<UserAuthMiddleware> _logger;

        public UserAuthMiddleware(ILogger<UserAuthMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled request exception was converted to an unauthorized response.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseModel
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = Messages.UserAccessFail
                }));
            }
        }

    }
}
