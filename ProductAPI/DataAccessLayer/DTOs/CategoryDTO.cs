

using DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
	public class CategoryDTO
	{
		public int CategoryId { get; set; }
		public int? ParentId { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = null!;
		public bool IsDeleted { get; set; }
        [StringLength(500, ErrorMessage = "Name cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public List<CategoryDTO> InverseParent { get; set; } = new List<CategoryDTO>();
    }
}
