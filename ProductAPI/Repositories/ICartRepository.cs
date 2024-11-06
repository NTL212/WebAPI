using ProductAPI.Models;

namespace ProductAPI.Repositories
{
	public interface ICartRepository
	{
		Task<Cart> GetActiveCartByUserId(int userId);
		Task<IEnumerable<CartItem>> GetCartItems(int cartId);
		Task<Cart> CreateCartIfNotExists(int userId);
		Task<bool> AddItemToCart(int cartId, CartItem cartItem);
		Task<bool> UpdateCartItem(int cartId, CartItem cartItem);
		Task<bool> PlusCartItem(int cartId, int cartItemId);
		Task<bool> MinusCartItem(int cartId, int cartItemId);
		Task<bool> RemoveItemFromCart(int cartId, int cartItemId);
		Task<bool> ClearCart(int cartId);
	}
}
