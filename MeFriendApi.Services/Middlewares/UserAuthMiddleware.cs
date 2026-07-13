using MeFriendApi.Domain;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using static MeFriendApi.Domain.Dto.Helpers.CommonDto;

namespace MeFriendApi.Services.Middlewares
{
    public class UserAuthMiddleware : IMiddleware
    {
        public UserAuthMiddleware()
        {
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseModel { StatusCode = 401, Message = Messages.UserAccessFail }));
                return;
            }
        }

    }
}
