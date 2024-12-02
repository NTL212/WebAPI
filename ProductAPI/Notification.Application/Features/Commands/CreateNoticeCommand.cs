using MediatR;
using Notification.Application.DTOs;


namespace Notification.Application.Features.Commands
{
    public class CreateNoticeCommand:IRequest<OrderNoticeDTO>
    {
        public OrderNoticeDTO OrderNotice { get; set; }
    }
}
