using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductAPI.DTOs;
using ProductAPI.Models;

namespace ProductAPI.Repositories
{
	public class OrderRepository:IOrderRepository
	{
		private readonly ProductCategoryContext _context;
		private readonly IMapper _mapper;

		public OrderRepository(ProductCategoryContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Tạo đơn hàng mới
		public async Task<Order> CreateOrderAsync(int userId)
		{
			var cart = await _context.Carts.Include(c=>c.CartItems).ThenInclude(ci=>ci.Product).FirstOrDefaultAsync(c => c.UserId == userId);
			if (cart == null || cart.CartItems.Count == 0)
				return null; // Giỏ hàng không tồn tại hoặc không có sản phẩm.
			var order = new Order
			{
				UserId = userId,
				OrderDate = DateTime.Now,
				Status = "Pending",
				TotalAmount = cart.CartItems.Sum(item=>item.Price * item.Quantity)
			};

			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			foreach (var item in cart.CartItems)
			{
				var product = await _context.Products.FirstOrDefaultAsync(c => c.ProductId == userId);
				var orderItem = _mapper.Map<OrderItem>(item);
				orderItem.OrderId = order.OrderId;
				product.Stock -= item.Quantity;

				if (product.Stock > 0)
				{
					_context.OrderItems.Add(orderItem);
					_context.Products.Update(product);
					_context.CartItems.Remove(item);
				}
			}

			await _context.SaveChangesAsync();
			return order;
		}

		// Lấy tất cả đơn hàng của người dùng
		public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
		{
			return await _context.Orders
				.Where(o => o.UserId == userId)
				.Include(o => o.OrderItems) // Bao gồm các mục đơn hàng
				.ToListAsync();
		}

		// Lấy đơn hàng theo OrderId
		public async Task<Order> GetOrderByIdAsync(int orderId)
		{
			return await _context.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefaultAsync(o => o.OrderId == orderId);
		}

		// Cập nhật trạng thái đơn hàng
		public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
		{
			var order = await _context.Orders.FindAsync(orderId);
			if (order == null)
				return false;

			order.Status = status;
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<IEnumerable<Order>> GetAll()
		{
			return await _context.Orders
				.ToListAsync();
		}
	}
}
