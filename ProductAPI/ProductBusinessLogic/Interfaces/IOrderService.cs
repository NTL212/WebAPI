using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using ProductDataAccess.ViewModels;
using System.Linq.Expressions;

namespace ProductBusinessLogic.Interfaces
{
    public interface IOrderService:IBaseService<OrderDTO>
    {
        Task<ResultVM> CreateOrderAsync(OrderDTO orderDto, List<CartItemDTO> cartItems);
        Task<List<OrderDTO>> GetOrdersByUserIdAsync(int userId);
        Task<List<OrderDTO>> GetSelectedOrders(List<int> selectedOrders);
        Task<PagedResult<OrderDTO>> GetPagedByUserAsync(int userId, int pageNumber, int pageSize);
        Task<PagedResult<OrderDTO>> GetOrderPagedWithSearch(int pageNumber, int pageSize, string searchKey);
        Task<OrderDTO> GetOrderById(int orderId);

        Task<bool> CancelOrderAsync(int orderId);
        Task<int> GetOrderCountByUserAsync(int userId);
        Task<bool> ConfirmOrders(List<int> selectedOrderIds);
    }
}
