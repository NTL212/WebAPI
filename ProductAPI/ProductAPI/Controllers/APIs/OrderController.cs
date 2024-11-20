using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;
using ProductDataAccess.DTOs;

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
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var result = await _orderRepository.CreateOrderAsync(order);
            if (result == null)
                return BadRequest("Could not create order.");

            // Ánh xạ lại Order sang OrderDTO
            var orderDTO = _mapper.Map<OrderDTO>(order);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = orderDTO.OrderId }, orderDTO);

        }

		// Lấy tất cả đơn hàng 
		[HttpGet]
		public async Task<IActionResult> GetAllOrder()
		{
			var orders = await _orderRepository.GetAllAsync();

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
            var order = await _orderRepository.GetByIdAsync(orderId);
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

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var updated = await _orderRepository.CancelOrderAsync(orderId);
            if (!updated)
                return BadRequest("Could not update order status.");

            return NoContent();
        }


    }
}
