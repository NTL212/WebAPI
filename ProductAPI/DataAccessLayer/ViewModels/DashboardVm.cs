using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class DashboardVm
    {
        public decimal TotalRevenueToday { get; set; }
        public decimal TotalRevenueMonth { get; set; }

        public int NewUserQuantityWeek { get; set; }
        public int NewUserQuantityQuarter { get; set; }
        public decimal RevenueGrowthDay { get; set; }
        public decimal RevenueGrowthMonth { get; set; }
        public decimal UserGrowthWeek { get; set; }
        public decimal UserGrowthQuarter { get; set; }


        public List<CustomerRevenueResult> CustomerRevenueResults { get; set; } =  new List<CustomerRevenueResult>();
        public List<ProductRevenueResult> ProductRevenueResults { get; set; } = new List<ProductRevenueResult>();
    }
}
