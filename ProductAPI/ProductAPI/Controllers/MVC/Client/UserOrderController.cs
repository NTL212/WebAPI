using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductDataAccess.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using Newtonsoft.Json;
using ProductDataAccess.Models.Request;

using ProductAPI.Services;
using Microsoft.Extensions.Caching.Distributed;
using ProductDataAccess.Models;
using System.Drawing.Printing;
using ProductBusinessLogic.Interfaces;
using ProductAPI.Helper;


namespace ProductAPI.Controllers.MVC.Client
{
    [JwtAuthorize("Customer")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class UserOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly RabbitMqService _rabbitMqService;
        private readonly IDistributedCache _distributedCache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public UserOrderController(IOrderRepository orderRepository, IVoucherRepository voucherRepository, IOrderService orderService, RabbitMqService rabbitMqService,   IDistributedCache distributedCache, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
            _rabbitMqService = rabbitMqService;
            _distributedCache = distributedCache;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(int userId, bool resetCached=false, int pageNumber = 1, string mess = null)
        {
            const int pageSize = 5;
            string cacheKey = $"UserOrders:{userId}:Page:{pageNumber}";
            if (resetCached)
            {
                CacheHelper helper = new CacheHelper();
                var pageCount = await _orderService.GetOrderCountByUserAsync(userId);
                await helper.InvalidateUserOrdersCache(userId, pageCount, _distributedCache);
            }
            PagedResult<OrderDTO> cachedPageResult;

            // Try to get cached data
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Deserialize cached JSON data to object
                cachedPageResult = JsonConvert.DeserializeObject<PagedResult<OrderDTO>>(cachedData);
            }
            else
            {
                // If not found in cache, fetch from DB
                var pageResult = await _orderRepository.GetPagedByUserAsync(userId, pageNumber, pageSize);

                // Map to DTO
                cachedPageResult = _mapper.Map<PagedResult<OrderDTO>>(pageResult);

                // Serialize data to JSON and store in Redis
                var serializedData = JsonConvert.SerializeObject(cachedPageResult);
                await _distributedCache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                });
            }

            // Set pagination info in ViewData
            ViewData["TotalPages"] = (int)Math.Ceiling(cachedPageResult.TotalRecords / (double)pageSize);
            ViewData["CurrentPage"] = pageNumber;
            ViewData["message"] = mess;

            return View(cachedPageResult);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            string cacheKey = $"UserOrders:{userId}:Order:{id}";
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);
            OrderDTO cachedResult;

            if (!string.IsNullOrEmpty(cachedData))
            {
                // Deserialize cached JSON data to object
                cachedResult = JsonConvert.DeserializeObject<OrderDTO>(cachedData);
            }
            else
            {
                // If not found in cache, fetch from DB
                var order = await _orderRepository.GetOrderById(id);
                Voucher voucher=null;
                if (order.VoucherId != null)
                {
                    voucher = await _voucherRepository.GetByIdAsync((int)order.VoucherId);
                }
                if (order == null)
                    return NotFound();

                // Ánh xạ từ Order sang OrderDTO
                cachedResult = _mapper.Map<OrderDTO>(order);
                if (voucher != null)
                {
                    cachedResult.Voucher = _mapper.Map<VoucherDTO>(voucher);
                }
              
                // Serialize data to JSON and store in Redis
                var serializedData = JsonConvert.SerializeObject(cachedResult);
                await _distributedCache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                });
            }

           
            return View(cachedResult);
        }

        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
           
            // Tạo RabbitMessage
            var rabbitMessage = new RabbitMessage
            {
                ActionType = "Cancel",  // Loại hành động (có thể là "Create", "Cancel", "Confirm")
                Payload = orderId         // Dữ liệu payload là đơn hàng
            };

            // Serialize RabbitMessage to JSON
            var rabbitMessageJson = JsonConvert.SerializeObject(rabbitMessage);

            _rabbitMqService.PublishOrderMessage(rabbitMessageJson);

            string cacheKey = $"UserOrders:{userId}:Order:{orderId}";
            await _distributedCache.RemoveAsync(cacheKey);

            TempData["SuccessMessage"] = "Your order has been cancled for processing!";
            return RedirectToAction("Index", new { userId=userId, resetCached = true });
        }
    }
}
