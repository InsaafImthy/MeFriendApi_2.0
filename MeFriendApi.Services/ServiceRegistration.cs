using MeFriendApi.Services.Infrastructure;
using MeFriendApi.Services.Interfaces;
using MeFriendApi.Services.Middlewares;
using MeFriendApi.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeFriendApi.Services
{
    public static class ServiceRegistration
    {
        public static void RegisterService(IServiceCollection services)
        {
            services.AddApplicationServices();
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddHttpContextAccessor();
            services.AddHttpClient(BusinessCentralDefaults.HttpClientName);
            services.AddTransient<UserAuthMiddleware>();
            services.AddScoped<IBusinessCentralCompanyContext, BusinessCentralCompanyContext>();
            services.AddSingleton<IBusinessCentralContinuationTokenService, BusinessCentralContinuationTokenService>();
            services.AddScoped<ID365CommonService, D365CommonService>();
            services.AddScoped<ICustomersService, CustomersService>();
            services.AddScoped<IItemMastersService, ItemMastersService>();
            services.AddScoped<ISalespersonsService, SalespersonsService>();
            services.AddScoped<IDimensionsService, DimensionsService>();
            services.AddScoped<ISalesOrdersService, SalesOrdersService>();
            services.AddScoped<ISalesInvoicesService, SalesInvoicesService>();

            return services;
        }
    }
}
