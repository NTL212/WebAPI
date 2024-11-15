using ProductDataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.ViewModels
{
    public  class VoucherCreateVM
    {
        public string Code { get; set; }
        public string VoucherType { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderValue { get; set; }
        public string? ProductId { get; set; }
        public int? CategoryId { get; set; }
        public int? MaxUsage { get; set; }

        public int? MaxPerUsage { get; set; }

        public string? GroupName { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public string? Status { get; set; }
        public int MaxUsesPerUser { get; set; }
        public DateTime ExpiryDate { get; set; }

        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }
}
