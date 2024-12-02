using Microsoft.Extensions.DependencyInjection;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories.Implementations;
using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ProductDataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ProductCategoryContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepoisitory, UserRepository>();
            services.AddScoped<IUserGroupRepository, UserGroupRepository>();
            services.AddScoped<IVoucherUserRepository, VoucherUserRepository>();
            services.AddScoped<IVoucherCampaignRepository, VoucherCampaignRepository>();
            services.AddScoped<IVoucherAssignmentRepository, VoucherAssignmentRepository>();


            return services;
        }
    }
}
