﻿using Microsoft.AspNetCore.Mvc;
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
using Microsoft.Extensions.Caching.Distributed;

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
        private readonly IDistributedCache _distributedCache;

        // Injecting the service in the controller's constructor
        public CartController(ICartService cartService, IMapper mapper, IOrderRepository orderRepository,
            IVoucherRepository voucherRepository, IVoucherUserRepository voucherUserRepository,
            IUserRepoisitory userRepository, IProductRepository productRepository,
            RabbitMqService rabbitMqService, IDistributedCache distributedCache)
        {
            _cartService = cartService;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _voucherUserRepository = voucherUserRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _rabbitMqService = rabbitMqService;
            _distributedCache = distributedCache;
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
                checkoutVM.voucherUserDTOs = _mapper.Map<List<VoucherUserDTO>>(voucherUsers);
                if (voucherAppiedId > 0)
                {
                    var productIds = cart.Select(x => (int)x.ProductId).ToList();
                    var check = await _voucherRepository.ValidateVoucher(voucherApplied, user, productIds, checkoutVM.total);
                    if (check.Success)
                    {
                        TempData["SuccessMessage"] = check.Message;
                        checkoutVM.voucherApplied = _mapper.Map<VoucherDTO>(voucherApplied);
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
                var checkoutVM = _mapper.Map<CheckoutVM>(orderDTO);
                return View(checkoutVM);
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


            _rabbitMqService.PublishOrderMessage(rabbitMessageJson);
            _cartService.ClearCart();
            TempData["SuccessMessage"] = "Your order has been submitted for processing!";
            return RedirectToAction("Index", "UserOrder", new { userId = userId, resetCached = true });
        }

        [HttpPost]
        public async Task<IActionResult> OrderResult([FromBody] ResultVM resultVM)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cart = _cartService.GetCart();
            var data = await _distributedCache.GetStringAsync("df32753a-e2d3-7127-6736-049d9ad9dc8d");
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
    }
}
