using DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class VoucherBaseVM
    {
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }

}
