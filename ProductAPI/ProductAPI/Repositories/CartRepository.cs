using Microsoft.EntityFrameworkCore;
using ProductDataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly ProductCategoryContext _context;

		public CartRepository(ProductCategoryContext context)
		{
			_context = context;
		}
		public async Task<Cart> CreateCartIfNotExists(int userId)
		{
			var existingCart = await _context.Carts
				.FirstOrDefaultAsync(c => c.UserId == userId);

			if (existingCart == null)
			{
				var newCart = new Cart
				{
					UserId = userId,
					CreatedDate = DateTime.Now,
					Total = 0
				};

				_context.Carts.Add(newCart);
				await _context.SaveChangesAsync();
				return newCart;
			}

			return existingCart;
		}

		// Lấy giỏ hàng đang hoạt động của người dùng
		public async Task<Cart> GetActiveCartByUserId(int userId)
		{
			return await _context.Carts
				.FirstOrDefaultAsync(c => c.UserId == userId);
		}

		// Lấy tất cả các mục trong giỏ hàng
		public async Task<IEnumerable<CartItem>> GetCartItems(int userId)
		{
			var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
			return await _context.CartItems.Include(ci=>ci.Product)
				.Where(ci => ci.CartId == cart.CartId)
				.ToListAsync();
		}

		// Thêm một mục vào giỏ hàng
		public async Task<bool> AddItemToCart(int userId, CartItem cartItem)
		{
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
			cartItem.CartId = cart.CartId;
            var cartItemExist = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == cartItem.ProductId);		
			var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
			if (cartItemExist == null)
			{
				if(product.Stock>=cartItem.Quantity)
					_context.CartItems.Add(cartItem);
				else
					return false;
			}
			else
			{
				cartItemExist.Quantity += cartItem.Quantity;
				cartItemExist.Price = cartItem.Price;
				if (product.Stock >= cartItemExist.Quantity)
					_context.CartItems.Update(cartItemExist);
				else
					return false;
			}
			
			return await _context.SaveChangesAsync() > 0;
		}

		// Cập nhật một mục trong giỏ hàng (cập nhật số lượng hoặc giá)
		public async Task<bool> UpdateCartItem(int cartId, CartItem cartItem)
		{
			var existingItem = await _context.CartItems
				.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.CartItemId == cartItem.CartItemId);
			var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
			if (existingItem == null)
				return false;

			existingItem.Quantity = cartItem.Quantity;
			existingItem.Price = cartItem.Price;
			if (product.Stock >= existingItem.Quantity)
				_context.CartItems.Update(existingItem);
			else
				return false;
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> PlusCartItem(int cartId, int cartItemId)
		{
			var existingItem = await _context.CartItems
				.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.CartItemId == cartItemId);
			var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == existingItem.ProductId);
			if (existingItem == null || product==null)
				return false;

			existingItem.Quantity +=1;
			if (product.Stock >= existingItem.Quantity)
				_context.CartItems.Update(existingItem);
			else
				return false;
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> MinusCartItem(int cartId, int cartItemId)
		{
			var existingItem = await _context.CartItems
				.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.CartItemId == cartItemId);
			var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == existingItem.ProductId);
			if (existingItem == null || product == null)
				return false;

			existingItem.Quantity -= 1;
			if (product.Stock >= existingItem.Quantity && existingItem.Quantity>0)
				_context.CartItems.Update(existingItem);
			else
				return false;
			return await _context.SaveChangesAsync() > 0;
		}

		// Xóa một mục khỏi giỏ hàng
		public async Task<bool> RemoveItemFromCart(int cartId, int cartItemId)
		{
			var cartItem = await _context.CartItems
				.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.CartItemId == cartItemId);

			if (cartItem == null)
				return false;

			_context.CartItems.Remove(cartItem);
			return await _context.SaveChangesAsync() > 0;
		}

		// Xóa toàn bộ mục khỏi giỏ hàng
		public async Task<bool> ClearCart(int cartId)
		{
			var cartItems = await _context.CartItems
				.Where(ci => ci.CartId == cartId)
				.ToListAsync();

			_context.CartItems.RemoveRange(cartItems);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}

