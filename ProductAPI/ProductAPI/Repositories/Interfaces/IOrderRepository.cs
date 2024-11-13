using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;

namespace ProductAPI.Repositories
{
	public interface IOrderRepository:IRepository<Order>
	{
		Task<Order> CreateOrderAsync(int userId, Order order);
		Task<PagedResult<Order>> GetPagedByUserAsync(int userId, int pageNumber, int pageSize);
		Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
		Task<bool> UpdateOrderStatusAsync(int orderId, string status);

        Task<Order> GetOrderById(int orderId);
    }
}
