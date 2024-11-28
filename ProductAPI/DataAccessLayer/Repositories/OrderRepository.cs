using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using DataAccessLayer.Models.Response;
using DataAccessLayer.ViewModels;
using DataAccessLayer.Interfaces;

namespace DataAccessLayer.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ProductCategoryContext context) : base(context)
        {
        }

        // Tạo đơn hàng mới
        public async Task<ResultVM> CreateOrderAsync(Order order)
        {
            try
            {
                List<OrderItem> list = new List<OrderItem>();
                order.OrderDate = DateTime.Now;
                order.Status = "Pending";

                if (order.VoucherId != null && order.VoucherId != 0)
                {
                    var voucherUser = await _context.VoucherUsers.FirstOrDefaultAsync(x => x.VoucherId == order.VoucherId && x.UserId == order.UserId);
                    voucherUser.TimesUsed += 1;
                    _context.VoucherUsers.Update(voucherUser);
                }

                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Set<Product>().FirstOrDefaultAsync(c => c.ProductId == item.ProductId);

                    product.Stock -= item.Quantity;

                    if (product.Stock < 0)
                    {
                        return new ResultVM(false, $"Product {product.ProductName} out of stock");
                    }
                    _context.Products.Update(product);
                }
                await _dbSet.AddAsync(order);
                if (await _context.SaveChangesAsync() > 0)
                {

                    return new ResultVM(true, "Place order successfully");
                }
                return new ResultVM(false, "Place order failed");
            }
            catch (Exception ex)
            {
                return new ResultVM(false, "Place order failed");
            }
        }


        // Lấy tất cả đơn hàng của người dùng
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems) // Bao gồm các mục đơn hàng
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetPagedByUserAsync(int userId, int pageNumber, int pageSize)
        {
            return await _dbSet
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _dbSet.Include(o => o.User).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return false;

            order.Status = "Canceled";

            if (order.VoucherId > 0)
            {
                var voucherUser = await _context.VoucherUsers.FindAsync(order.VoucherId);
                if (voucherUser.TimesUsed > 0 && voucherUser.TimesUsed <= voucherUser.Quantity)
                {
                    voucherUser.TimesUsed -= 1;
                }
            }

            foreach (var o in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(o.ProductId);
                if (product != null)
                {
                    product.Stock += o.Quantity;
                }
            }
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ConfirmOrders(List<int> selectedOrderIds)
        {
            var orders = await _dbSet.Where(o => selectedOrderIds.Contains(o.OrderId)).ToListAsync();

            orders.ForEach(o => o.Status = "Confirmed");
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
