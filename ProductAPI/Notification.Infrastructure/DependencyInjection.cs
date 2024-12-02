using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Interfaces.Repositories;
using Notification.Infrastructure.Data;
using Notification.Infrastructure.Profiles;
using Notification.Infrastructure.Respositories;


namespace Notification.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            //Mapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Add MongoDB Context
            services.AddSingleton<MongoDbContext>();

            // Repositories
            services.AddScoped<IOrderNoticeRepository, OrderNoticeRepository>();


            return services;
        }
    }
}
