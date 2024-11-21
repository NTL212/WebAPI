
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductDataAccess.Repositories;
using ProductDataAccess.DTOs;
using Newtonsoft.Json;
using ProductDataAccess.Models.Request;
using RabbitMQ.Client;
using System.Text;
using static MassTransit.ValidationResultExtensions;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;
        public AdminOrderController(IOrderRepository orderRepository, IVoucherRepository voucherRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(int page = 1, string mess = null, string searchText = "")
        {
            var orders = await _orderRepository.GetPagedWithIncludeSearchAsync(page, 10, p => p.PhoneNumber.ToLower().Contains(searchText.ToLower()));
            ViewBag.Message = mess;
            return View(orders);
        }

        public async Task<IActionResult> Detail(int id, string mess = null)
        {
            var order = await _orderRepository.GetOrderById(id);
            var orderDto = _mapper.Map<OrderDTO>(order);

            if (order.VoucherId != null)
            {
                var voucher = await _voucherRepository.GetByIdAsync((int)order.VoucherId);
                if (voucher != null)
                {
                    orderDto.Voucher = _mapper.Map<VoucherDTO>(voucher);
                }
            }

            if (order == null)
                return NotFound();
            ViewBag.Message = mess;
            return View(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrders(List<int> selectedOrderIds, int page=1)
        {
            var updated = await _orderRepository.ConfirmOrders(selectedOrderIds);
            if (updated)
            {
                TempData["SuccessMessage"] = "Confirm orders successfull";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to confirm order";
            }
            if (selectedOrderIds.Count >1 )
            {
                return RedirectToAction("Index", new { page = page });
            }
            return RedirectToAction("Detail", new { id = selectedOrderIds.ElementAt(0)});
        }
    }
}
