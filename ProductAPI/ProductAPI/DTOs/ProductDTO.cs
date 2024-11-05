namespace ProductAPI.DTOs
{
	public class ProductDTO
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; } = null!;

		public int? CategoryId { get; set; }

		public decimal Price { get; set; }

		public int? Stock { get; set; }
		public bool? IsDeleted { get; set; }
	}
}
