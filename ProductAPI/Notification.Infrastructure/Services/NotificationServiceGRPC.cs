using AutoMapper;
using Grpc.Core;
using MediatR;
using Notification.Application.DTOs;
using Notification.Application.Features.Commands;
using Notification.Application.Features.Queries;
using Notification.Application.Interfaces.Services;

namespace Notification.Infrastructure.Services
{
    public class NotificationServiceGRPC : NotificationService.NotificationServiceBase
    {
        private readonly IMediator _mediator;
        private readonly IOrderNoticeService _orderNoticeService;
        private readonly IMapper _mapper;
        public NotificationServiceGRPC(IOrderNoticeService orderNoticeService, IMapper mapper, IMediator mediator)
        {
            _orderNoticeService = orderNoticeService;
            _mediator = mediator;
            _mapper = mapper;
        }

        public override async Task<NotifyList> GetNotifyList(Empty request, ServerCallContext context)
        {
            var notieDtos = await _orderNoticeService.GetAllOrderNotices();
            var notices = _mapper.Map<List<Notice>>(notieDtos);
            var response = new NotifyList();
            response.Notices.AddRange(notices);
            return response;
        }

        public override async Task<Notice> GetNoticeById(NoticeIdRequest request, ServerCallContext context)
        {
            var notice = await _mediator.Send(new GetNoticeQuery { Id = request.Id });
            if (notice == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Notice not found"));

            return _mapper.Map<Notice>(notice);
        }

        public override async Task<NoticeResponse> AddNotice(Notice request, ServerCallContext context)
        {
            var notice = await _mediator.Send(new CreateNoticeCommand { OrderNotice = _mapper.Map<OrderNoticeDTO>(request) });
            return new NoticeResponse { Status = "Success", Message = "Notice added successfully" };
        }

        public override async Task<NoticeResponse> UpdateNotice(Notice request, ServerCallContext context)
        {
            var notice = await _orderNoticeService.GetOrderNoticeById(request.Id);
            if (notice == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Notice not found"));

            notice.Title = request.Title;
            notice.UserId = request.UserId;
            notice.OrderId = request.OrderId;
            notice.Message = request.Message;

            var noticeUpdated = await _orderNoticeService.UpdateOrderNotice(notice);

            return new NoticeResponse { Status = "Success", Message = "Notice updated successfully" };
        }

        public override async Task<NoticeResponse> DeleteNotice(NoticeIdRequest request, ServerCallContext context)
        {
            var notice = await _orderNoticeService.GetOrderNoticeById(request.Id);
            if (notice == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Notice not found"));

            var result = await _orderNoticeService.DeleteOrderNotice(notice.Id);

            return new NoticeResponse { Status = result == true ? "Success" : "Failed", Message = "Notice deleted successfully" };
        }

        public override async Task<NoticeResponse> UpdateNoticeIsSeen(NoticeIdRequest request, ServerCallContext context)
        {
            var notice = await _mediator.Send(new GetNoticeQuery { Id = request.Id});
            if (notice == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Notice not found"));

            var result = await _mediator.Send(new UpdateNoticeIsSeenCommand { Id = request.Id});
            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Notice not found"));

            return new NoticeResponse { Status = result == true ? "Success" : "Failed", Message = "Notice updated successfully" };
        }

        public override async Task<NotifyList> GetNotifyByOrderList(IdRequest request, ServerCallContext context)
        {
            var notieDtos = await _orderNoticeService.GetAllOrderNoticesByOrder(request.Id);
            var notices = _mapper.Map<List<Notice>>(notieDtos);
            var response = new NotifyList();
            response.Notices.AddRange(notices);
            return response;
        }

        public override async Task<NotifyList> GetNotifyByUserList(IdRequest request, ServerCallContext context)
        {
            var notieDtos = await _orderNoticeService.GetAllOrderNoticesByUser(request.Id);
            var notices = _mapper.Map<List<Notice>>(notieDtos);
            var response = new NotifyList();
            response.Notices.AddRange(notices);
            return response;
        }
    }
}
