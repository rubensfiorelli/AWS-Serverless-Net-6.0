using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Register.Application.Services;
using Register.Core.Contracts;
using Register.Data.Repositories;

namespace Register.IoC
{
    public static class InfraModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddServices()
                .AddRepositories();


            return services;
        }
        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IOrderService), typeof(OrderService));


            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));


            return services;
        }
    }
}
