using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class ProductRevenueResult
    {
        public int ProductId { get; set; }

        public string? ImgName { get; set; } 
        public string? ProductName { get; set; } 
        public decimal TotalRevenue { get; set; }
        public int TotalQuantitySold { get; set; }
    }
}
