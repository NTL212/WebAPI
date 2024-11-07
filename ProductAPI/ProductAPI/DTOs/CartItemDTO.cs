using ProductAPI.Models;

namespace ProductAPI.DTOs
{
	public class CartItemDTO
	{
		public int? CartItemId { get; set; }
		public int CartId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }  // Giá tại thời điểm thêm vào giỏ hàng

		public ProductDTO? Product { get; set; }
	}
}
