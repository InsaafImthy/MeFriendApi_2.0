using MeFriendApi.Domain;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
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
                var path = context.Request.Path.Value?.ToLower() ?? "";
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseModel { StatusCode = 401, Message = Messages.UserAccessFail }));
                return;
            }
        }

    }
}
