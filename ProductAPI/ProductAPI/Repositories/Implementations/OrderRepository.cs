using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Services;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;

namespace ProductAPI.Repositories
{
	public class OrderRepository:Repository<Order>, IOrderRepository
	{
		private readonly IMapper _mapper;
		private readonly ICartService _cartService;
		public OrderRepository(ProductCategoryContext context, IMapper mapper, ICartService cartService) : base(context)
		{
			_mapper = mapper;
			_cartService = cartService;
		}

		// Tạo đơn hàng mới
		public async Task<Order> CreateOrderAsync(int userId, Order order)
		{
			List<OrderItem> list = new List<OrderItem>();
			var cart = _cartService.GetCart();
			if (cart == null)
				return null; // Giỏ hàng không tồn tại hoặc không có sản phẩm.
			order.UserId = userId;
			order.OrderDate = DateTime.Now;
			order.Status = "Pending";

			_dbSet.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
			{
				var product = await _context.Set<Product>().FirstOrDefaultAsync(c => c.ProductId == item.ProductId);
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                product.Stock -= item.Quantity;

				if (product.Stock > 0)
				{
					await _context.OrderItems.AddAsync(orderItem);
					_context.Products.Update(product);
					_cartService.RemoveFromCart((int)item.ProductId);
				}
			}

			await _context.SaveChangesAsync();
			return order;
		}

		// Lấy tất cả đơn hàng của người dùng
		public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
		{
			return await _dbSet
				.Where(o => o.UserId == userId)
				.Include(o => o.OrderItems) // Bao gồm các mục đơn hàng
				.ToListAsync();
		}

		public async Task<PagedResult<Order>> GetPagedByUserAsync(int userId, int pageNumber, int pageSize)
		{
			var totalRecords = await _dbSet.CountAsync();
			var items = await _dbSet
				.Where (o => o.UserId == userId)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<Order>
			{
				Items = items,
				TotalRecords = totalRecords,
				PageNumber = pageNumber,
				PageSize = pageSize
			};
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
			return await _dbSet.Include(o=>o.User).Include(o=>o.OrderItems).ThenInclude(oi=>oi.Product).FirstOrDefaultAsync(o=>o.OrderId==orderId);
        }
    }
}
