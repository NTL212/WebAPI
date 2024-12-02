using MediatR;
using Notification.Application.DTOs;


namespace Notification.Application.Features.Queries
{
    public class GetNoticesQuery : IRequest<List<OrderNoticeDTO>> { }
}
