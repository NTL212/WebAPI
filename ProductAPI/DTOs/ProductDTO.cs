namespace ProductAPI.DTOs
{
	public class ProductDTO
	{
		public int ProductId { get; set; }

		public string ProductName { get; set; } = string.Empty;

		public int? CategoryId { get; set; }

		public decimal Price { get; set; }

		public int? Stock { get; set; }

		public string? ImgName { get; set; }

		public DateTime? CreatedAt { get; set; }

		public bool? IsDeleted { get; set; }
	}
}
