using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Interfaces.Services;
using Notification.Application.Profiles;
using Notification.Application.Services;
using System.Reflection;

namespace Notification.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Mapper
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Services
            services.AddScoped<IOrderNoticeService, OrderNoticeService>();

            return services;
        }
    }
}
