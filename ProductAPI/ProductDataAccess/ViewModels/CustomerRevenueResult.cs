using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.ViewModels
{
    public class CustomerRevenueResult
    {
        public int UserId { get; set; }

        public string Email {  get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
    }

}
