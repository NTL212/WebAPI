using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using ProductDataAccess.ViewModels;

namespace ProductAPI.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ProductCategoryContext _context;
        public StatisticsService(ProductCategoryContext context)
        {
            _context = context;
        }


        public async Task<decimal> CalculateTotalRevenue(DateTime startDate, DateTime endDate)
        {
            // Lọc đơn hàng trong khoảng thời gian
            var filteredOrders = _context.Orders.Where(order => order.OrderDate >= startDate && order.OrderDate <= endDate).ToList();

            // Tính tổng doanh thu và tổng số đơn hàng
            var totalRevenue = filteredOrders.Sum(order => order.TotalAmount);
            int totalOrders = filteredOrders.Count;

            return totalRevenue ?? 0;
        }

        public async Task<List<CustomerRevenueResult>> CalculateCustomerRevenue(DateTime startDate, DateTime endDate)
        {
            var statistics = await (from o in _context.Orders
                                    join od in _context.OrderItems on o.OrderId equals od.OrderId
                                    join u in _context.Users on o.UserId equals u.UserId
                                    where o.OrderDate >= startDate && o.OrderDate <= endDate
                                    group new { o.UserId, od.Quantity, od.Price, u.Email } by o.UserId into g
                                    select new CustomerRevenueResult
                                    {
                                        UserId = g.Key,
                                        TotalRevenue = g.Sum(x => x.Quantity * x.Price),
                                        Email = g.FirstOrDefault().Email,
                                        TotalOrders = g.Count()
                                    })
                                  .OrderByDescending(x => x.TotalRevenue)
                                  .ToListAsync();
            
            return statistics;
        }

        public async Task<List<ProductRevenueResult>> CalculateProductRevenue(DateTime startDate, DateTime endDate)
        {
            var statistics = await (from p in _context.Products
                                    join od in _context.OrderItems on p.ProductId equals od.ProductId
                                    join o in _context.Orders on od.OrderId equals o.OrderId
                                    where o.OrderDate >= startDate && o.OrderDate <= endDate
                                    group new { od, p } by p.ProductId into g
                                    select new ProductRevenueResult
                                    {
                                        ProductId = g.Key,
                                        ImgName = g.FirstOrDefault().p.ImgName,
                                        ProductName = g.FirstOrDefault().p.ProductName,
                                        TotalQuantitySold = g.Sum(x => x.od.Quantity),
                                        TotalRevenue = g.Sum(x => x.od.Quantity * x.od.Price),
                                    }).ToListAsync();


            return statistics;
        }

        public async Task<int> CalculateNewUserQuantity(DateTime startDate, DateTime endDate)
        {
            var userCount = await _context.Users.Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate).CountAsync();
            return userCount;
        }

        public async Task<DashboardVm> GetDashboardVm()
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
            var totalRevenueToday = await CalculateTotalRevenue(startDateToday, endDateToday);
            var totalRevenueMonth = await CalculateTotalRevenue(firstDayOfMonth, lastDayOfMonth);
            var totalRevenueYesterday = await CalculateTotalRevenue(startDateToday.AddDays(-1), endDateToday.AddDays(-1));
            var totalRevenueLastMonth = await CalculateTotalRevenue(firstDayOfMonth.AddMonths(-1), lastDayOfMonth.AddMonths(-1));

            // Tính tỷ lệ tăng trưởng doanh thu
            var revenueGrowthDay = CalculateGrowth(totalRevenueToday, totalRevenueYesterday);
            var revenueGrowthMonth = CalculateGrowth(totalRevenueMonth, totalRevenueLastMonth);

            // Tính doanh thu theo khách hàng và sản phẩm
            var listCustomerRevenue = await CalculateCustomerRevenue(firstDayOfMonth, lastDayOfMonth);
            var listProductRevenue = await CalculateProductRevenue(firstDayOfMonth, lastDayOfMonth);

            // Tính số lượng người dùng mới
            var newUserQuantityWeek = await CalculateNewUserQuantity(startOfWeek, endOfWeek);
            var newUserQuantityQuarter = await CalculateNewUserQuantity(startOfQuarter, endOfQuarter);
            var newUserQuantityLastWeek = await CalculateNewUserQuantity(startOfWeek.AddDays(-7), endOfWeek.AddDays(-7));
            var newUserQuantityLastQuarter = await CalculateNewUserQuantity(startOfQuarter.AddMonths(-3), endOfQuarter.AddMonths(-3));

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

            return dashboardVm;
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
