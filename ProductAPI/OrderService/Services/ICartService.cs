using ProductDataAccess.Models;
using System.Security.Cryptography;

namespace OrderService.Services
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        bool AddToCart(CartItem item);
        bool RemoveFromCart(int productId);
        void SaveCart(List<CartItem> cart);
        bool UpdateCartItem(int productId, int newQuantity);
        decimal GetTotalPrice();
        int GetTotalQuantity();
        bool IsProductInCart(int productId);
        void ClearCart();


    }
}
