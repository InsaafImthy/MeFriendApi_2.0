using MeFriendApi.Services.Interfaces;
using MeFriendApi.Services.Middlewares;
using MeFriendApi.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeFriendApi.Services
{
    public class ServiceRegistration
    {
        public static void RegisterService(IServiceCollection services)
        {
            services.AddHttpClient(nameof(D365CommonService));
            services.AddTransient<UserAuthMiddleware>();
            services.AddScoped<ID365CommonService, D365CommonService>();
            services.AddScoped<ICustomersService, CustomersService>();
            services.AddScoped<IItemMastersService, ItemMastersService>();
            services.AddScoped<ISalespersonsService, SalespersonsService>();
            services.AddScoped<IDimensionsService, DimensionsService>();
            services.AddScoped<ISalesOrdersService, SalesOrdersService>();
            services.AddScoped<ISalesInvoicesService, SalesInvoicesService>();
        }
    }
}
