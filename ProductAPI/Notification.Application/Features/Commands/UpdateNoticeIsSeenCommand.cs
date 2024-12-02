

using MediatR;


namespace Notification.Application.Features.Commands
{
    public class UpdateNoticeIsSeenCommand : IRequest<bool>
    {
        public string Id { get; set; }
    }
}
