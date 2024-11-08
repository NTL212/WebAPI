using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Repositories;
using ProductAPI.DTOs;

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        // Tạo đơn hàng mới
        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateOrder(int userId, OrderDTO orderDto)
        {
            var newOrder = _mapper.Map<Order>(orderDto);
            var order = await _orderRepository.CreateOrderAsync(userId, newOrder);
            if (order == null)
                return BadRequest("Could not create order.");

            // Ánh xạ lại Order sang OrderDTO
            var orderDTO = _mapper.Map<OrderDTO>(order);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderId }, orderDTO);
        }

		// Lấy tất cả đơn hàng 
		[HttpGet]
		public async Task<IActionResult> GetAllOrder()
		{
			var orders = await _orderRepository.GetAll();

			// Ánh xạ từ các Order sang OrderDTO
			var ordersDTO = _mapper.Map<IEnumerable<OrderDTO>>(orders);
			return Ok(ordersDTO);
		}


		// Lấy tất cả đơn hàng của một người dùng
		[HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            // Ánh xạ từ các Order sang OrderDTO
            var ordersDTO = _mapper.Map<IEnumerable<OrderDTO>>(orders);
            return Ok(ordersDTO);
        }

        // Lấy chi tiết đơn hàng theo OrderId
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            // Ánh xạ từ Order sang OrderDTO
            var orderDTO = _mapper.Map<OrderDTO>(order);
            return Ok(orderDTO);
        }

        // Cập nhật trạng thái đơn hàng
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string status)
        {
            var updated = await _orderRepository.UpdateOrderStatusAsync(orderId, status);
            if (!updated)
                return BadRequest("Could not update order status.");

            return NoContent();
        }
    }
}
