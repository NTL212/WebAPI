

namespace Notification.Domain.Entities
{
    public class OrderNotice
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public string Message { get; set; }
        public bool IsSeen { get; set; }
        public DateTime Created { get; set; }   

    }
}
