using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Models;
using ProductAPI.Services;
using ProductDataAccess.DTOs;
using AutoMapper;
using ProductAPI.Repositories;

namespace ProductAPI.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        // Injecting the service in the controller's constructor
        public CartController(ICartService cartService, IMapper mapper, IOrderRepository orderRepository)
        {
            _cartService = cartService;
            _mapper = mapper;
            _orderRepository = orderRepository;
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
        public async Task<IActionResult> CheckoutAllCart()
        {
            var cart = _cartService.GetCart();
            if (cart.Count > 0)
            {
                return View(cart);
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
