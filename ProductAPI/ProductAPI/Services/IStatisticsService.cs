using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.ViewModels;

namespace ProductAPI.Services
{
    public interface IStatisticsService
    {
        Task<decimal> CalculateTotalRevenue(DateTime startDate, DateTime endDate);
        Task<int> CalculateNewUserQuantity(DateTime startDate, DateTime endDate);
        Task<List<CustomerRevenueResult>> CalculateCustomerRevenue(DateTime startDate, DateTime endDate);
        Task<List<ProductRevenueResult>> CalculateProductRevenue(DateTime startDate, DateTime endDate);
        
        Task<DashboardVm> GetDashboardVm();
    }
}
