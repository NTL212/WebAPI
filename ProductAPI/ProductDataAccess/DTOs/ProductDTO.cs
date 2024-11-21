using ProductDataAccess.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductDataAccess.DTOs
{
	public class ProductDTO
	{
		public int ProductId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string ProductName { get; set; } = string.Empty;

		public int CategoryId { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

		public string? ImgName { get; set; }

		public DateTime CreatedAt { get; set; }
        public CategoryDTO? Category { get; set; }
        public bool IsDeleted { get; set; }
	}
}
