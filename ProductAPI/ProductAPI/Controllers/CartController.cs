using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Models;
using ProductAPI.Services;
using ProductDataAccess.DTOs;
using AutoMapper;
using ProductAPI.Repositories;
using ProductDataAccess.ViewModels;
using ProductAPI.Repositories.Interfaces;
using Newtonsoft.Json.Linq;

namespace ProductAPI.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IVoucherUserRepository _voucherUserRepository;

        // Injecting the service in the controller's constructor
        public CartController(ICartService cartService, IMapper mapper, IOrderRepository orderRepository, IVoucherRepository voucherRepository, IVoucherUserRepository voucherUserRepository)
        {
            _cartService = cartService;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _voucherUserRepository = voucherUserRepository;
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
        public async Task<IActionResult> ApplyVoucher (int voucherAppiedId = 0)
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

        [HttpGet]
        public async Task<IActionResult> CheckoutAllCart(int voucherAppiedId=0)
        {
            var cart = _cartService.GetCart();
            var userId = HttpContext.Session.GetInt32("UserId");
            var voucherUsers = await _voucherUserRepository.GetAllWithPredicateIncludeAsync(v=>v.UserId==userId && v.Status==true, v=>v.Voucher);

            var voucherApplied = await _voucherRepository.GetByIdAsync(voucherAppiedId);

            var checkoutVM = new CheckoutVM();

            if (cart.Count > 0)
            {
                checkoutVM.cartItems = cart;
                checkoutVM.voucherApplied = _mapper.Map<VoucherDTO>(voucherApplied);
                checkoutVM.voucherUserDTOs = _mapper.Map<List<VoucherUserDTO>>(voucherUsers);
                ViewBag.VoucherAppliedId = voucherAppiedId;
                return View(checkoutVM);
            }
            else
            {
                ViewData["Message"] = "Please shopping";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CheckoutAllCart([FromBody] OrderDTO orderDTO)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var order = _mapper.Map<Order>(orderDTO);
            order.UserId = (int)userId;
            order.VoucherId = orderDTO.VoucherAppliedId;
            var orderCreated = await _orderRepository.CreateOrderAsync((int)userId, order);
            if (orderCreated != null)
            {
                return Ok(orderCreated);

            }
            else
            {
                return BadRequest();
            }
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
