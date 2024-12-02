using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.Models;
using ProductAPI.Services;
using ProductDataAccess.DTOs;
using ProductAPI.Filters;
using ProductBusinessLogic.Interfaces;
namespace ProductAPI.Controllers.MVC.Client
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICheckoutService _checkoutService;

        public CartController(ICartService cartService, ICheckoutService checkoutService, ICacheService cacheService)
        {
            _cartService = cartService;
            _checkoutService = checkoutService;
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
        [HttpGet]
        public async Task<IActionResult> CheckoutAllCart(int voucherAppiedId = 0)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var checkoutVM = await _checkoutService.PrepareCheckoutViewModel(userId, voucherAppiedId);
            return View(checkoutVM);
        }

        [JwtAuthorize("Customer")]
        [HttpPost]
        public async Task<IActionResult> CheckoutAllCart(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid) return View(orderDTO);

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            bool success = await _checkoutService.ProcessOrder(orderDTO, userId);

            if (success)
            {
                TempData["SuccessMessage"] = "Your order has been processed!";
                return RedirectToAction("Index", "UserOrder", new {userId = userId, resetCached =true});
            }

            TempData["ErrorMessage"] = "Error processing your order!";
            return View(orderDTO);
        }

        private JsonResult JsonResponse(bool success, object data)
        {
            return Json(new { success, redirectUrl = Url.Action("Index"), data });
        }
    }
}
