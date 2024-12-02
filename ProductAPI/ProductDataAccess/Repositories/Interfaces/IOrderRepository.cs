using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories
{
	public interface IOrderRepository:IRepository<Order>
	{
        Task<IEnumerable<Order>> GetPagedByUserAsync(int userId, int pageNumber, int pageSize);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order> GetOrderById(int orderId);

        Task<Order> CreateOrder(Order order);
    }
}
