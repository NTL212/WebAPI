using Microsoft.AspNetCore.Mvc;
using ProductAPI.Services;

namespace ProductAPI.Controllers.MVC.Client
{
    public class NotificationController : Controller
    {
        private readonly INotificationGRPCService _notificationGRPCService;

        public NotificationController(INotificationGRPCService notificationGRPCService)
        {
            _notificationGRPCService = notificationGRPCService;
        }

        public async Task<IActionResult> GetNotifications(int userId = 0)
        {
            var notices = await _notificationGRPCService.GetNoticesByUserAsync(userId);
            return PartialView("_UserNotices",notices);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsSeen([FromBody]string id)
        {
            var result = await _notificationGRPCService.UpdateNoticeIsSeenAsync(id);
            return Ok(result);
        }
    }
}
