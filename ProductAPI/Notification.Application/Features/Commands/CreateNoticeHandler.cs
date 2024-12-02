using AutoMapper;
using MediatR;
using Notification.Application.DTOs;
using Notification.Application.Interfaces.Services;

namespace Notification.Application.Features.Commands
{
    public class CreateNoticeHandler : IRequestHandler<CreateNoticeCommand, OrderNoticeDTO>
    {
        private readonly IOrderNoticeService _orderNoticeService;

        public CreateNoticeHandler(IOrderNoticeService orderNoticeService)
        {
            _orderNoticeService = orderNoticeService;
        }

        public async Task<OrderNoticeDTO> Handle(CreateNoticeCommand request, CancellationToken cancellationToken)
        {
           var noticeCreated = await _orderNoticeService.CreateOrderNotice(request.OrderNotice);
            return noticeCreated;
        }
    }
}
