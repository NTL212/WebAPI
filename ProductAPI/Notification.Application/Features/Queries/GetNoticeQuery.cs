using MediatR;
using Notification.Application.DTOs;


namespace Notification.Application.Features.Queries
{
    public class GetNoticeQuery: IRequest<OrderNoticeDTO>
    {
        public string Id { get; set; }
    }
}
