using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductAPI.Repositories;
using ProductDataAccess.DTOs;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public AdminOrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<IActionResult> Index(int page=1, string mess=null)
        {
            var orders = await _orderRepository.GetPagedAsync(page, 10);
            ViewBag.Message = mess;
            return View(orders);
        }

        public async Task<IActionResult> Detail(int id, string mess=null) 
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound();
            ViewBag.Message = mess;
            return View(order);
        }

        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            string status = "Confirmed";
            string message = "Failed";
            var updated = await _orderRepository.UpdateOrderStatusAsync(orderId, status);
            if (updated)
            {
                message = "Success";
            }
            return RedirectToAction("Detail", new { id = orderId, mess = message });
        }
    }
}
