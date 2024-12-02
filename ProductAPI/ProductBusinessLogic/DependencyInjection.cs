using Microsoft.Extensions.DependencyInjection;
using ProductBusinessLogic.Interfaces;
using ProductBusinessLogic.Services;



namespace ProductBusinessLogic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IVoucherUserService, VoucherUserService>();
            services.AddScoped<IVoucherCampaignService, VoucherCampaignService>();
            services.AddScoped<IVoucherAssignmentService, VoucherAssignmentService>();


            return services;
        }
    }
}
