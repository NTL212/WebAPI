using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.ViewModels
{
    public class DistributeVoucherVM
    {
        public int VoucherId { get; set; }
        public List<int>  UserIds { get; set; }

        public int Quantity { get; set; }
    }
}
