
using MediatR;
using Notification.Application.Interfaces.Services;

namespace Notification.Application.Features.Commands
{
    internal class UpdateNoticeIsSeenHandler : IRequestHandler<UpdateNoticeIsSeenCommand, bool>
    {
        private readonly IOrderNoticeService _orderNoticeService;

        public UpdateNoticeIsSeenHandler(IOrderNoticeService orderNoticeService)
        {
            _orderNoticeService = orderNoticeService;
        }

        public async Task<bool> Handle(UpdateNoticeIsSeenCommand request, CancellationToken cancellationToken)
        {
            var result = await _orderNoticeService.UpdateOrderNoticeIsSeen(request.Id);
            return result;
        }
    }
}
