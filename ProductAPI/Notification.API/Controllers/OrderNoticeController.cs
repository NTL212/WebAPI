using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.DTOs;
using Notification.Application.Features.Queries;
using Notification.Application.Interfaces.Services;

namespace Notification.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderNoticeController : ControllerBase
    {
        private readonly IOrderNoticeService _orderNoticeService;
        private readonly IMediator _mediator;
        public OrderNoticeController(IOrderNoticeService orderNoticeService, IMediator mediator)
        {
            _mediator = mediator;
            _orderNoticeService = orderNoticeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notices = await _mediator.Send(new GetNoticesQuery());
            return Ok(notices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _mediator.Send(new GetNoticeQuery { Id = id});
                if (product == null) return NotFound();
                return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> Create(OrderNoticeDTO noticeDTO)
        {
            var result = await _orderNoticeService.CreateOrderNotice(noticeDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, OrderNoticeDTO noticeDTO)
        {
            var noticeUpdated = await _orderNoticeService.UpdateOrderNotice(noticeDTO);
            if (noticeUpdated == null)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _orderNoticeService.DeleteOrderNotice(id);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}
