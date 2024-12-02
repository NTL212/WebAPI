using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;
using ProductDataAccess.DTOs;
using ProductBusinessLogic.Interfaces;
using ProductAPI.Services;

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly INotificationGRPCService _notificationGRPCService;

        public OrderController(IOrderService orderService, INotificationGRPCService notificationGRPCService)
        {
            _orderService = orderService;
            _notificationGRPCService = notificationGRPCService;
        }

        // Tạo đơn hàng mới
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO order)
        {
            var result = await _orderService.CreateOrderAsync(order);
            if (result == null)
                return BadRequest("Could not create order.");

            if (result.IsSuccess)
            {
                var notice = new Notice
                {
                    UserId = result.Order.UserId,
                    OrderId = result.Order.OrderId,
                    Title = "Place Order Successfully",
                    Message = result.Message
                };
                var message  = await _notificationGRPCService.AddNoticeAsync(notice);
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }

        }

		// Lấy tất cả đơn hàng 
		[HttpGet]
		public async Task<IActionResult> GetAllOrder()
		{
			var orders = await _orderService.GetAllAsync();

			return Ok(orders);
		}


		// Lấy tất cả đơn hàng của một người dùng
		[HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        // Lấy chi tiết đơn hàng theo OrderId
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var updated = await _orderService.CancelOrderAsync(orderId);
            if (!updated)
                return BadRequest("Could not update order status.");

            return NoContent();
        }


    }
}
