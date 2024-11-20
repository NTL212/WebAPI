using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ProductDataAccess.Repositories;
using ProductDataAccess.Models;
using System.Text.Json;

namespace ProductAPI.Services
{
    public class CartService:ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string CartSessionKey = "CartSession";
        private readonly IProductRepository _productRepository;

        public CartService(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
        }

        // Lấy giỏ hàng từ session
        public List<CartItem> GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString(CartSessionKey);
            if (cartJson == null)
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        // Thêm sản phẩm vào giỏ hàng
        public bool AddToCart(CartItem item)
        {
            // Lấy thông tin sản phẩm
            var product = _productRepository.GetByIdAsync((int)item.ProductId).Result;
            if (product == null || item.Quantity > product.Stock)
            {
                return false;
            }

            // Lấy giỏ hàng hiện tại và kiểm tra sản phẩm đã tồn tại hay chưa
            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem != null)
            {
                // Cập nhật số lượng nếu đủ tồn kho
                var newQuantity = existingItem.Quantity + item.Quantity;
                if (newQuantity > product.Stock)
                {
                    return false;
                }
                existingItem.Quantity = newQuantity;
            }
            else
            {
                // Thêm sản phẩm vào giỏ nếu số lượng hợp lệ
                item.Product = product;
                cart.Add(item);
            }

            // Lưu giỏ hàng và trả về kết quả thành công
            SaveCart(cart);
            return true;
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public bool UpdateCartItem(int productId, int newQuantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            var product  = _productRepository.GetByIdAsync(productId).Result;
            if (item != null && product.Stock >=newQuantity)
            {
                item.Quantity = newQuantity > 0 ? newQuantity : 0;
                if (item.Quantity == 0)
                {
                    cart.Remove(item);
                }
                SaveCart(cart);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public bool RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);            
            }
            else
                return false;
            SaveCart(cart);
			return true;
		}

        // Tính tổng giá trị của giỏ hàng
        public decimal GetTotalPrice()
        {
            var cart = GetCart();
            return cart.Sum(item => item.Price * item.Quantity);
        }

        // Đếm tổng số lượng sản phẩm trong giỏ hàng
        public int GetTotalQuantity()
        {
            var cart = GetCart();
            return cart.Sum(item => item.Quantity);
        }

        // Kiểm tra sản phẩm có tồn tại trong giỏ hàng hay không
        public bool IsProductInCart(int productId)
        {
            var cart = GetCart();
            return cart.Any(i => i.ProductId == productId);
        }

        // Làm rỗng giỏ hàng
        public void ClearCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Remove(CartSessionKey);
        }

        // Lưu giỏ hàng vào session
        public void SaveCart(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(CartSessionKey, cartJson);
        }
    }
}
