using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.ViewModels
{
    public class ProductCategoryViewModel
    {
        public PagedResult<ProductDTO> Products { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public List<int> SelectedCategories { get; set; } = new List<int>();
    }
}
