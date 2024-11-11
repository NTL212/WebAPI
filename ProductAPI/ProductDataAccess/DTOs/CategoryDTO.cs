

using ProductDataAccess.Models;

namespace ProductDataAccess.DTOs
{
	public class CategoryDTO
	{
		public int CategoryId { get; set; }
		public int? ParentId { get; set; }	
		public string CategoryName { get; set; } = null!;
		public bool IsDeleted { get; set; }
		public string? Description { get; set; }

        public List<CategoryDTO> InverseParent { get; set; } = new List<CategoryDTO>();
    }
}
