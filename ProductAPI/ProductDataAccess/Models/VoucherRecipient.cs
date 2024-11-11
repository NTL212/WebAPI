using System;
using System.Collections.Generic;

namespace ProductDataAccess.Models;

public partial class VoucherRecipient
{
    public int Id { get; set; }

    public int? VoucherId { get; set; }

    public int? UserId { get; set; }

    public int? GroupId { get; set; }

    public bool? IsUsed { get; set; }

    public virtual UserGroup? Group { get; set; }

    public virtual User? User { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
