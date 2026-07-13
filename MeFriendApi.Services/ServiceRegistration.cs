using MeFriendApi.Services.Interfaces;
using MeFriendApi.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeFriendApi.Services
{
    public class ServiceRegistration
    {
        public static void RegisterService(IServiceCollection services)
        {
            services.AddScoped<ID365CommonService, D365CommonService>();
            services.AddScoped<ICustomersService, CustomersService>();
        }
    }
}
