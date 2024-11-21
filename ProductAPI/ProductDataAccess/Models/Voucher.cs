using System;
using System.Collections.Generic;

namespace ProductDataAccess.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string Code { get; set; } = null!;

    public decimal? DiscountValue { get; set; }

    public string? DiscountType { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? Status { get; set; }

    public int? MaxUsage { get; set; }

    public int? UsedCount { get; set; }

    public string? VoucherType { get; set; }

    public string? Conditions { get; set; }

    public virtual ICollection<OrderVoucher> OrderVouchers { get; set; } = new List<OrderVoucher>();

    public virtual ICollection<VoucherAssignment> VoucherAssignments { get; set; } = new List<VoucherAssignment>();

    public virtual ICollection<VoucherUser> VoucherUsers { get; set; } = new List<VoucherUser>();
}
