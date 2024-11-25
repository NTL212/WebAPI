
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;
using ProductDataAccess.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static NuGet.Packaging.PackagingConstants;

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
    }
}
