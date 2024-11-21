using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Models;
using ProductAPI.Services;
using ProductDataAccess.DTOs;
using AutoMapper;
using ProductDataAccess.Repositories;
using ProductDataAccess.ViewModels;
using ProductDataAccess.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using ProductAPI.Filters;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models.Request;

namespace ProductAPI.Controllers.MVC.Client
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IVoucherUserRepository _voucherUserRepository;
        private readonly IUserRepoisitory _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly RabbitMqService _rabbitMqService;

        // Injecting the service in the controller's constructor
        public CartController(ICartService cartService, IMapper mapper, IOrderRepository orderRepository,
            IVoucherRepository voucherRepository, IVoucherUserRepository voucherUserRepository,
            IUserRepoisitory userRepository, IProductRepository productRepository,
            RabbitMqService rabbitMqService)
        {
            _cartService = cartService;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _voucherUserRepository = voucherUserRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _rabbitMqService = rabbitMqService;
        }

        // Get the cart and display it along with the total price and quantity
        [HttpGet]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            ViewBag.TotalPrice = _cartService.GetTotalPrice();
            ViewBag.TotalQuantity = _cartService.GetTotalQuantity();
            return View(cart);
        }

        // API to get cart item count
        [HttpGet]
        public IActionResult GetCartCount()

        {
            var count = _cartService.GetCart().Count;
            return JsonResponse(true, count);
        }

        // API to add an item to the cart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, decimal price)
        {
            var item = new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = price
            };
            bool added = _cartService.AddToCart(item);
            return JsonResponse(added, null);
        }

        // API to update an item in the cart
        [HttpPost]
        public IActionResult UpdateCartItem(int productId, int quantity)
        {
            bool updated = _cartService.UpdateCartItem(productId, quantity);
            return updated ? JsonResponse(true, null) : BadRequest();
        }

        // API to remove an item from the cart
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            bool removed = _cartService.RemoveFromCart(productId);
            return JsonResponse(removed, null);
        }

        // API to clear the entire cart
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }


        [JwtAuthorize("Customer")]
        [ServiceFilter(typeof(ValidateTokenAttribute))]
        [HttpGet]
        public async Task<IActionResult> CheckoutAllCart(int voucherAppiedId = 0)
        {
            var cart = _cartService.GetCart();
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _userRepository.GetByIdWithIncludeAsync(u => u.UserId == userId, u => u.Group);
            var voucherUsers = await _voucherUserRepository.GetAllWithPredicateIncludeAsync(v => v.UserId == userId && v.Status == true, v => v.Voucher);

            var voucherApplied = await _voucherRepository.GetByIdAsync(voucherAppiedId);

            var checkoutVM = new CheckoutVM();

            if (cart.Count > 0)
            {
                checkoutVM.cartItems = cart;
                checkoutVM.voucherApplied = _mapper.Map<VoucherDTO>(voucherApplied);
                checkoutVM.voucherUserDTOs = _mapper.Map<List<VoucherUserDTO>>(voucherUsers);
                if (voucherAppiedId > 0)
                {
                    var productIds = cart.Select(x => (int)x.ProductId).ToList();
                    var check = await _voucherRepository.ValidateVoucher(voucherApplied, user, productIds, checkoutVM.total);
                    if (check.Success)
                    {
                        TempData["SuccessMessage"] = check.Message;
                        ViewBag.VoucherAppliedId = voucherAppiedId;
                        ViewBag.VoucherCode = voucherApplied.Code;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = check.Message;
                    }
                }
                return View(checkoutVM);

            }
            else
            {
                ViewData["Message"] = "Please shopping";
                return RedirectToAction("Index");
            }

        }

        [JwtAuthorize("Customer")]
        [ServiceFilter(typeof(ValidateTokenAttribute))]
        [HttpPost]
        public async Task<IActionResult> CheckoutAllCart(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid) { 

            }

            var cart = _cartService.GetCart();
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _userRepository.GetByIdAsync((int)userId);
            var order = _mapper.Map<Order>(orderDTO);
            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                order.OrderItems.Add(orderItem);
            }

            order.UserId = (int)userId;
            order.VoucherId = orderDTO.VoucherAppliedId;



            // Tạo RabbitMessage
            var rabbitMessage = new RabbitMessage
            {
                ActionType = "Create",  // Loại hành động (có thể là "Create", "Cancel", "Confirm")
                Payload = order         // Dữ liệu payload là đơn hàng
            };

            // Serialize RabbitMessage to JSON
            var rabbitMessageJson = JsonConvert.SerializeObject(rabbitMessage);

            // Send order data to RabbitMQ
            //var factory = new ConnectionFactory
            //{
            //    HostName = "localhost",
            //    AutomaticRecoveryEnabled = true, // Tự động khôi phục kết nối
            //    NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // Khoảng thời gian thử lại
            //};
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
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

            TempData["SuccessMessage"] = "Your order has been submitted for processing!";
            return RedirectToAction("Index", "UserOrder", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> OrderResult([FromBody] ResultVM resultVM)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (resultVM.IsSuccess)
            {
                TempData["SuccessMessage"] = resultVM.Message;
                _cartService.ClearCart();
                return RedirectToAction("Index", "UserOrder", new { userId });
            }
            else
            {
                TempData["ErrorMessage"] = resultVM.Message;
            }           
            return RedirectToAction("Index");
        }


        // Helper method to standardize the JSON response
        private JsonResult JsonResponse(bool success, object data)
        {
            return Json(new
            {
                success,
                redirectUrl = Url.Action("Index", "Cart"),
                data
            });
        }


    }
}
