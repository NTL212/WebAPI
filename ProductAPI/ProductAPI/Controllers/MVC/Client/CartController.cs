using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Models;
using ProductAPI.Services;
using ProductDataAccess.DTOs;
using AutoMapper;
using ProductAPI.Repositories;
using ProductDataAccess.ViewModels;
using ProductAPI.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using ProductAPI.Filters;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

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

        // Injecting the service in the controller's constructor
        public CartController(ICartService cartService, IMapper mapper, IOrderRepository orderRepository,
            IVoucherRepository voucherRepository, IVoucherUserRepository voucherUserRepository,
            IUserRepoisitory userRepository)
        {
            _cartService = cartService;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _voucherUserRepository = voucherUserRepository;
            _userRepository = userRepository;
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

        [HttpGet]
        public async Task<IActionResult> ApplyVoucher(int voucherAppiedId = 0)
        {
            var cart = _cartService.GetCart();
            var userId = HttpContext.Session.GetInt32("UserId");
            var voucherUsers = await _voucherUserRepository.GetAllWithPredicateIncludeAsync(v => v.UserId == userId && v.Status == true, v => v.Voucher);
            var voucherApplied = await _voucherRepository.GetByIdAsync(voucherAppiedId);

            var checkoutVM = new CheckoutVM();

            if (cart.Count > 0)
            {
                checkoutVM.cartItems = cart;
                checkoutVM.voucherApplied = _mapper.Map<VoucherDTO>(voucherApplied);
                checkoutVM.voucherUserDTOs = _mapper.Map<List<VoucherUserDTO>>(voucherUsers);
                return Ok(checkoutVM);
            }
            else
            {
                return BadRequest();
            }

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
            //var userId = HttpContext.Session.GetInt32("UserId");
            //var order = _mapper.Map<Order>(orderDTO);
            //order.UserId = (int)userId;
            //order.VoucherId = orderDTO.VoucherAppliedId;
            //var orderCreated = await _orderRepository.CreateOrderAsync((int)userId, order);
            //if (orderCreated != null)
            //{
            //    TempData["SuccessMessage"] = "Place order successfull";
            //}
            //else
            //{
            //    TempData["ErrorMessage"] = "Failed to place order";
            //}
            //return RedirectToAction("Index", "UserOrder", new { userId });

            var userId = HttpContext.Session.GetInt32("UserId");
            var order = _mapper.Map<Order>(orderDTO);
            order.UserId = (int)userId;
            order.VoucherId = orderDTO.VoucherAppliedId;

            // Serialize order data to JSON
            var orderJson = JsonConvert.SerializeObject(order);

            // Send order data to RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Update with RabbitMQ host if needed
            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                await channel.QueueDeclareAsync(queue: "OrderQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(orderJson);

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "OrderQueue",
                    mandatory: false, // Chuyển tham số properties vào đây
                    body: body
                );
            }

            TempData["SuccessMessage"] = "Your order has been submitted for processing!";
            return RedirectToAction("Index", "UserOrder", new { userId });
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
