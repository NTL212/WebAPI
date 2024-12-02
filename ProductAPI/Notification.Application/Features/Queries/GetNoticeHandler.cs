using AutoMapper;
using MediatR;
using Notification.Application.DTOs;
using Notification.Application.Interfaces.Services;

namespace Notification.Application.Features.Queries
{
    public class GetNoticeHandler : IRequestHandler<GetNoticeQuery, OrderNoticeDTO>
    {
        private readonly IOrderNoticeService _orderNoticeService;
        public GetNoticeHandler(IOrderNoticeService orderNoticeService)
        {
            _orderNoticeService = orderNoticeService; 
        }

        public async Task<OrderNoticeDTO> Handle(GetNoticeQuery request, CancellationToken cancellationToken)
        {
            var notice = await _orderNoticeService.GetOrderNoticeById(request.Id);
            return notice;
        }
    }
}
