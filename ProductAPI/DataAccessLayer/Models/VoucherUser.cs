using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class VoucherUser
{
    public int VoucherUserId { get; set; }

    public int VoucherId { get; set; }

    public int UserId { get; set; }

    public int Quantity { get; set; }

    public int TimesUsed { get; set; }

    public DateTime? DateAssigned { get; set; }

    public bool? Status { get; set; }

    public virtual Voucher Voucher { get; set; } = null!;
}
