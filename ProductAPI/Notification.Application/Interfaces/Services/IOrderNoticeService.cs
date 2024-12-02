using Notification.Application.DTOs;

namespace Notification.Application.Interfaces.Services
{
    public interface IOrderNoticeService
    {
        Task<List<OrderNoticeDTO>> GetAllOrderNotices();
        Task<List<OrderNoticeDTO>> GetAllOrderNoticesByUser(int userId);
        Task<List<OrderNoticeDTO>> GetAllOrderNoticesByOrder(int orderId);
        Task<OrderNoticeDTO> GetOrderNoticeById(string id);
        Task<OrderNoticeDTO> CreateOrderNotice(OrderNoticeDTO orderNoticeDTO);
        Task<bool> UpdateOrderNoticeIsSeen(string id);
        Task<OrderNoticeDTO> UpdateOrderNotice(OrderNoticeDTO orderNoticeDTO);
        Task<bool> DeleteOrderNotice(string id);
    }
}
