using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using Microsoft.Extensions.Caching.Distributed;
using ProductBusinessLogic.Interfaces;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IVoucherService _voucherService;
        private readonly IDistributedCache _distributedCache;
        public AdminOrderController(IOrderService orderService, IVoucherService voucherService, IDistributedCache distributedCache)
        {
            _orderService = orderService;
            _voucherService = voucherService;
            _distributedCache = distributedCache;
        }
        public async Task<IActionResult> Index(int page = 1, string searchText = "")
        {
            var orders = await _orderService.GetOrderPagedWithSearch(page, 10, searchText);
            return View(orders);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetOrderById(id);

            if (order.VoucherAppliedId != null)
            {
                var voucher = await _voucherService.GetByIdAsync((int)order.VoucherAppliedId);
                if (voucher != null)
                {
                    order.Voucher = voucher;
                }
            }

            if (order == null)
                return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrders(List<int> selectedOrderIds, int page = 1)
        {
            if (!selectedOrderIds.Any())
            {
                TempData["ErrorMessage"] = "No orders selected.";
                return RedirectToAction("Index", new { page });
            }

            try
            {
                var result = await _orderService.ConfirmAndInvalidateCacheAsync(selectedOrderIds);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Orders confirmed successfully.";
                    return selectedOrderIds.Count > 1
                        ? RedirectToAction("Index", new { page })
                        : RedirectToAction("Detail", new { id = selectedOrderIds.First() });
                }
                else
                {
                    TempData["ErrorMessage"] = result.ErrorMessage;
                    return RedirectToAction("Index", new { page });
                }
            }
            catch (Exception ex)
            {
                // Log exception here
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Index", new { page });
            }
        }

    }
}
