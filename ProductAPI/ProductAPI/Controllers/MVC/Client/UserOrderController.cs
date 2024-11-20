using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductDataAccess.Repositories;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using Newtonsoft.Json;
using ProductDataAccess.Models.Request;
using ProductDataAccess.Models;
using System.Text;
using RabbitMQ.Client;


namespace ProductAPI.Controllers.MVC.Client
{
    [JwtAuthorize("Customer")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class UserOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;

        public UserOrderController(IOrderRepository orderRepository, IVoucherRepository voucherRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int userId, int pageNumber = 1, string mess = null)
        {
            var pageSize = 5;
            var UserId = (int)HttpContext.Session.GetInt32("UserId");

            // Lấy dữ liệu phân trang từ repository
            var pageResult = await _orderRepository.GetPagedByUserAsync(UserId, pageNumber, pageSize);

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

            // Send order data to RabbitMQ
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            using (var connection = factory.CreateConnection())
            using (var channel =  connection.CreateModel())
            {
                channel.QueueDeclare(queue: "OrderQueue2",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(rabbitMessageJson);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "OrderQueue2",
                    basicProperties: null,
                    body: body
                );
            }

            TempData["SuccessMessage"] = "Your order has been cancled for processing!";
            return RedirectToAction("Index", new { userId });
        }
    }
}
