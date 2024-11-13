using AutoMapper;
using Microsoft.AspNetCore.Mvc; 
using ProductAPI.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;


namespace ProductAPI.Controllers
{
    public class UserOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public UserOrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int userId, int pageNumber=1, string mess = null)
        {
			var pageSize = 5;

			// Lấy dữ liệu phân trang từ repository
			var pageResult = await _orderRepository.GetPagedByUserAsync(userId, pageNumber, pageSize);

			// Ánh xạ từ PageResult<Order> sang PageResult<OrderDTO>
			var pageResultDTO = _mapper.Map<PagedResult<OrderDTO>>(pageResult);

			// Truyền dữ liệu vào ViewData để phân trang
			ViewData["TotalPages"] = (int)Math.Ceiling(pageResult.TotalRecords / (double)pageSize);
			ViewData["CurrentPage"] = pageNumber;
            ViewData["message"] = mess;

			return View(pageResultDTO);
		}

        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound();

            // Ánh xạ từ Order sang OrderDTO
            var orderDTO = _mapper.Map<OrderDTO>(order);
            return View(order);
        }

        public async Task<IActionResult> CancelOrder(int orderId)
        {
            string status = "Cancelled";
            string message = "Failed";
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            var updated = await _orderRepository.UpdateOrderStatusAsync(orderId, status);
            if (updated)
            {
				message = "Success";
			}
            return RedirectToAction("Index", new {userid = userId, mess = message});
        }
    }
}
