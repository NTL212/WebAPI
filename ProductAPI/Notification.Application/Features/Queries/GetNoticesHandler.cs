using MediatR;
using Notification.Application.DTOs;
using Notification.Application.Interfaces.Services;

namespace Notification.Application.Features.Queries
{
    public class GetNoticesHandler : IRequestHandler<GetNoticesQuery, List<OrderNoticeDTO>>
    {
        private readonly IOrderNoticeService _orderNoticeService;

        public GetNoticesHandler(IOrderNoticeService orderNoticeService)
        {
            _orderNoticeService = orderNoticeService;
        }

        public async Task<List<OrderNoticeDTO>> Handle(GetNoticesQuery request, CancellationToken cancellationToken)
        {
            var notices = await _orderNoticeService.GetAllOrderNotices();
            return notices;
        }
    }
}
