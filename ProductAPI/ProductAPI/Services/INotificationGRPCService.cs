
namespace ProductAPI.Services
{
    public interface INotificationGRPCService
    {
        Task<string> AddNoticeAsync(Notice notice);
        Task<string> DeleteNoticeAsync(string id);
        Task<Notice> GetNoticeByIdAsync(string id);
        Task<List<Notice>> GetNoticesAsync();
        Task<List<Notice>> GetNoticesByUserAsync(int userId);
        Task<List<Notice>> GetNoticesByOrderAsync(int orderId);
        Task<string> UpdateNoticeAsync(Notice notice);
        Task<string> UpdateNoticeIsSeenAsync(string id);


    }
}