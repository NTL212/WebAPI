using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.Models
{
    public class VoucherCondition
    {
        public decimal MaxDiscountAmount { get; set; }
        public decimal MinOrderValue { get; set; }
        public string GroupName { get; set; }
        public List<int> ProductId { get; set; }
    }
}
