using ProductAPI.Models;

namespace ProductAPI.Repositories
{
	public interface IOrderRepository
	{
		Task<Order> CreateOrderAsync(int userId);
		Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
		Task<IEnumerable<Order>> GetAll ();
		Task<Order> GetOrderByIdAsync(int orderId);
		Task<bool> UpdateOrderStatusAsync(int orderId, string status);
	}
}
