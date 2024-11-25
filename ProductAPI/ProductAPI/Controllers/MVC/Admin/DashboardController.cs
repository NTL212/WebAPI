using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductAPI.Services;
using ProductDataAccess.Repositories;
using ProductDataAccess.ViewModels;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class DashboardController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IMapper _mapper;

        public DashboardController(IStatisticsService statisticsService, IMapper mapper)
        {
            _statisticsService = statisticsService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            // Khởi tạo các mốc thời gian
            var currentDate = DateTime.Now;
            var startDateToday = DateTime.Today;
            var endDateToday = DateTime.Today.AddDays(1).AddSeconds(-1);

            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);

            var startOfWeek = GetStartOfWeek(currentDate);
            var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

            var (startOfQuarter, endOfQuarter) = GetQuarterDates(currentDate);

            // Tính doanh thu
            var totalRevenueToday = await _statisticsService.CalculateTotalRevenue(startDateToday, endDateToday);
            var totalRevenueMonth = await _statisticsService.CalculateTotalRevenue(firstDayOfMonth, lastDayOfMonth);
            var totalRevenueYesterday = await _statisticsService.CalculateTotalRevenue(startDateToday.AddDays(-1), endDateToday.AddDays(-1));
            var totalRevenueLastMonth = await _statisticsService.CalculateTotalRevenue(firstDayOfMonth.AddMonths(-1), lastDayOfMonth.AddMonths(-1));

            // Tính tỷ lệ tăng trưởng doanh thu
            var revenueGrowthDay = CalculateGrowth(totalRevenueToday, totalRevenueYesterday);
            var revenueGrowthMonth = CalculateGrowth(totalRevenueMonth, totalRevenueLastMonth);

            // Tính doanh thu theo khách hàng và sản phẩm
            var listCustomerRevenue = await _statisticsService.CalculateCustomerRevenue(firstDayOfMonth, lastDayOfMonth);
            var listProductRevenue = await _statisticsService.CalculateProductRevenue(firstDayOfMonth, lastDayOfMonth);

            // Tính số lượng người dùng mới
            var newUserQuantityWeek = await _statisticsService.CalculateNewUserQuantity(startOfWeek, endOfWeek);
            var newUserQuantityQuarter = await _statisticsService.CalculateNewUserQuantity(startOfQuarter, endOfQuarter);
            var newUserQuantityLastWeek = await _statisticsService.CalculateNewUserQuantity(startOfWeek.AddDays(-7), endOfWeek.AddDays(-7));
            var newUserQuantityLastQuarter = await _statisticsService.CalculateNewUserQuantity(startOfQuarter.AddMonths(-3), endOfQuarter.AddMonths(-3));

            // Tính tỷ lệ tăng trưởng người dùng
            var userGrowthWeek = CalculateGrowth(newUserQuantityWeek, newUserQuantityLastWeek);
            var userGrowthQuarter = CalculateGrowth(newUserQuantityQuarter, newUserQuantityLastQuarter);

            DashboardVm dashboardVm = new DashboardVm();

            dashboardVm.TotalRevenueToday = totalRevenueToday;
            dashboardVm.TotalRevenueMonth = totalRevenueMonth;
            dashboardVm.ProductRevenueResults = listProductRevenue;
            dashboardVm.CustomerRevenueResults = listCustomerRevenue;
            dashboardVm.NewUserQuantityWeek = newUserQuantityWeek;
            dashboardVm.NewUserQuantityQuarter = newUserQuantityQuarter;
            dashboardVm.RevenueGrowthDay = revenueGrowthDay;
            dashboardVm.RevenueGrowthMonth = revenueGrowthMonth;
            dashboardVm.UserGrowthWeek = userGrowthWeek;
            dashboardVm.UserGrowthQuarter = userGrowthQuarter;

            return View(dashboardVm);
        }

        // Hàm tiện ích tính tỷ lệ tăng trưởng
        private decimal CalculateGrowth(decimal current, decimal previous)
        {
            return previous != 0 ? Math.Round((current - previous) * 100 / previous, 0) : 0;
        }

        // Hàm tiện ích lấy ngày đầu tuần
        private DateTime GetStartOfWeek(DateTime date)
        {
            return date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday).Date;
        }

        // Hàm tiện ích lấy ngày đầu và cuối quý
        private (DateTime startOfQuarter, DateTime endOfQuarter) GetQuarterDates(DateTime date)
        {
            int currentQuarter = (date.Month - 1) / 3 + 1;
            var startOfQuarter = new DateTime(date.Year, (currentQuarter - 1) * 3 + 1, 1);
            var endOfQuarter = startOfQuarter.AddMonths(3).AddSeconds(-1);
            return (startOfQuarter, endOfQuarter);
        }
    }
}
