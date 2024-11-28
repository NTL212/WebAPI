using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class OrderVoucher
{
    public int OrderVoucherId { get; set; }

    public int? OrderId { get; set; }

    public int? VoucherId { get; set; }

    public decimal? DiscountApplied { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
