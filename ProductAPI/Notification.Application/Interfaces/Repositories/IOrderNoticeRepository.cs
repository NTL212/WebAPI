using Notification.Domain.Entities;


namespace Notification.Application.Interfaces.Repositories
{
    public interface IOrderNoticeRepository
    {
        Task<IEnumerable<OrderNotice>> GetAllAsync();
        Task<IEnumerable<OrderNotice>> GetAllByUserAsync(int userId);

        Task<IEnumerable<OrderNotice>> GetAllByOrderAsync(int orderId);

        Task<OrderNotice> GetByIdAsync(string id);
        Task<OrderNotice> AddAsync(OrderNotice notice);
        Task<OrderNotice> UpdateAsync(OrderNotice notice);
        Task DeleteAsync(string id);

        Task SaveChanges();
    }
}
