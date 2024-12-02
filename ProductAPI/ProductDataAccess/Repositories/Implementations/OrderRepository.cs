using Microsoft.EntityFrameworkCore;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ProductCategoryContext context) : base(context)
        {
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

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _dbSet.Include(o => o.User).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _dbSet.AddAsync(order);
            return order;
        }
    }
}
