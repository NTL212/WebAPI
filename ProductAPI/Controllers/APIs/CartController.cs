using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.DTOs;
using ProductAPI.Filters;
using ProductAPI.Models;
using ProductAPI.Repositories;

namespace ProductAPI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(ICartRepository cartRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy giỏ hàng đang hoạt động của người dùng
        [HttpGet("active")]
        public async Task<ActionResult<CartDTO>> GetActiveCart()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cart = await _cartRepository.GetActiveCartByUserId((int)userId);

            if (cart == null)
                return NotFound("No active cart found for the user.");

            var cartDto = _mapper.Map<CartDTO>(cart);
            return Ok(cartDto);
        }

        // Lấy tất cả các mục trong giỏ hàng
        [HttpGet("{cartId}/items")]
        public async Task<ActionResult<IEnumerable<CartItemDTO>>> GetCartItems(int cartId)
        {
            var cartItems = await _cartRepository.GetCartItems(cartId);
            var cartItemsDto = _mapper.Map<IEnumerable<CartItemDTO>>(cartItems);
            return Ok(cartItemsDto);
        }

        // Thêm một mục vào giỏ hàng
        [HttpPost("{cartId}/items")]
        public async Task<ActionResult> AddItemToCart(int cartId, CartItemDTO cartItemDto)
        {
            var cartItem = _mapper.Map<CartItem>(cartItemDto);

            if (await _cartRepository.AddItemToCart(cartId, cartItem))
                return Ok("Item added to cart.");

            return BadRequest("Failed to add item to cart.");
        }

        // Cập nhật một mục trong giỏ hàng
        [HttpPut("{cartId}/items/{itemId}")]
        public async Task<ActionResult> UpdateCartItem(int cartId, int itemId, CartItemDTO cartItemDto)
        {
            if (itemId != cartItemDto.CartItemId)
                return BadRequest("Item ID mismatch.");

            var cartItem = _mapper.Map<CartItem>(cartItemDto);

            if (await _cartRepository.UpdateCartItem(cartId, cartItem))
                return NoContent();

            return NotFound("Item not found in cart.");
        }

        //Cộng số lượng item
        [HttpPut("{cartId}/items/{itemId}/plus")]
        public async Task<ActionResult> PlusCartItem(int cartId, int itemId)
        {
            if (await _cartRepository.PlusCartItem(cartId, itemId))
                return NoContent();

            return NotFound("Item not found in cart.");
        }

        //Trừ số lượng item
        [HttpPut("{cartId}/items/{itemId}/minus")]
        public async Task<ActionResult> MinusCartItem(int cartId, int itemId)
        {
            if (await _cartRepository.MinusCartItem(cartId, itemId))
                return NoContent();

            return NotFound("Item not found in cart.");
        }

        // Xóa một mục khỏi giỏ hàng
        [HttpDelete("{cartId}/items/{itemId}")]
        public async Task<ActionResult> RemoveItemFromCart(int cartId, int itemId)
        {
            if (await _cartRepository.RemoveItemFromCart(cartId, itemId))
                return NoContent();

            return NotFound("Item not found in cart.");
        }

        // Xóa toàn bộ mục khỏi giỏ hàng
        [HttpDelete("{cartId}/clear")]
        public async Task<ActionResult> ClearCart(int cartId)
        {
            if (await _cartRepository.ClearCart(cartId))
                return NoContent();

            return NotFound("Cart not found or already empty.");
        }
    }
}
