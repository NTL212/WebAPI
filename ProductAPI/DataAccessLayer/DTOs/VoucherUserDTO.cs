using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class VoucherUserDTO
    {
        public int VoucherUserId { get; set; }

        public int VoucherId { get; set; }

        public int UserId { get; set; }

        public int Quantity { get; set; }

        public int TimesUsed { get; set; }

        public virtual VoucherDTO Voucher { get; set; } = null!;
    }
}
