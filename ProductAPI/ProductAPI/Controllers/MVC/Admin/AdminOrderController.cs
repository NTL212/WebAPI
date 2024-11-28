using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using Microsoft.Extensions.Caching.Distributed;
using ProductAPI.Helper;
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
        public AdminOrderController(IOrderService orderService, IVoucherService voucherService, IDistributedCache distributedCache, IMapper mapper)
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
        public async Task<IActionResult> ConfirmOrders(List<int> selectedOrderIds, int page=1)
        {
            var updated = await _orderService.ConfirmOrders(selectedOrderIds);
            if (updated)
            {
                TempData["SuccessMessage"] = "Confirm orders successfull";
                var orders = await _orderService.GetSelectedOrders(selectedOrderIds);
                CacheHelper helper = new CacheHelper();
                foreach (var item in orders)
                {
                    var pageCount = await _orderService.GetOrderCountByUserAsync(item.UserId);
                    await helper.InvalidateUserOrdersCache(item.UserId, pageCount, _distributedCache);
                }            
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
