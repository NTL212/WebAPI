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

        public UserOrderController(IOrderRepository orderRepository, IVoucherRepository voucherRepository, RabbitMqService rabbitMqService,   IDistributedCache distributedCache, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
            _rabbitMqService = rabbitMqService;
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index(int userId, int pageNumber = 1, string mess = null)
        {
            const int pageSize = 5;
            string cacheKey = $"UserOrders:{userId}:Page:{pageNumber}";
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


        public async Task InvalidateUserOrdersCache(int userId)
        {
            // Xóa cache cho tất cả các trang của người dùng (giả sử bạn biết số lượng trang)
            int pageCount = await GetTotalPageCountForUser(userId); // Giả định bạn có cách lấy số trang
            for (int pageNumber = 1; pageNumber <= pageCount; pageNumber++)
            {
                string cacheKey = $"UserOrders:{userId}:Page:{pageNumber}";
                await _distributedCache.RemoveAsync(cacheKey); // Xóa cache cho từng trang
            }
        }

        private async Task<int> GetTotalPageCountForUser(int userId)
        {
            // Giả sử bạn có phương thức trả về tổng số đơn hàng
            var orders = await _orderRepository.GetPagedByUserAsync(userId, 1, 5);
            const int pageSize = 5;
            return orders.TotalPages;
        }

        public async Task<IActionResult> Detail(int id)
        {
          

            var order = await _orderRepository.GetOrderById(id);
            var voucher = await _voucherRepository.GetByIdAsync((int)order.VoucherId);
            if (order == null)
                return NotFound();

            // Ánh xạ từ Order sang OrderDTO
            var orderDTO = _mapper.Map<OrderDTO>(order);
            orderDTO.Voucher = _mapper.Map<VoucherDTO>(voucher);
            return View(orderDTO);
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

            //// Send order data to RabbitMQ
            //var factory = new ConnectionFactory
            //{
            //    HostName = "localhost"
            //};
            //using (var connection = factory.CreateConnection())
            //using (var channel =  connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "OrderQueue2",
            //                         durable: false,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    var body = Encoding.UTF8.GetBytes(rabbitMessageJson);

            //    channel.BasicPublish(
            //        exchange: "",
            //        routingKey: "OrderQueue2",
            //        basicProperties: null,
            //        body: body
            //    );
            //}
            _rabbitMqService.PublishOrderMessage(rabbitMessageJson);

            await InvalidateUserOrdersCache((int)userId);

            TempData["SuccessMessage"] = "Your order has been cancled for processing!";
            return RedirectToAction("Index", new { userId });
        }
    }
}
