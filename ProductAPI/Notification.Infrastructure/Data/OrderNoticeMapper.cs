using Notification.Domain.Entities;

namespace Notification.Infrastructure.Data
{
    public static class OrderNoticeMapper
    {
        public static MongoOrderNotice ToMongoModel(OrderNotice entity)
        {
            return new MongoOrderNotice
            {
                Id = entity.Id,
                UserId = entity.UserId,
                OrderId = entity.OrderId,
                Title = entity.Title,
                Message = entity.Message,
            };
        }

        public static OrderNotice ToDomainEntity(MongoOrderNotice document)
        {
            return new OrderNotice
            {
                Id = document.Id,
                UserId = document.UserId,
                OrderId = document.OrderId,
                Title = document.Title,
                Message = document.Message,
            };
        }
    }

}
