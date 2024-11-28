using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class AdminCategoryEditVM
    {
        public CategoryDTO Category { get; set; }
        public CategoryDTO ParentCategory { get; set; }
        public List<CategoryDTO> ParentCategories { get; set; }
    }
}
