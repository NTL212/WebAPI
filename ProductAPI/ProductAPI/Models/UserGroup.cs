using System;
using System.Collections.Generic;

namespace ProductAPI.Models;

public partial class UserGroup
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<VoucherRecipient> VoucherRecipients { get; set; } = new List<VoucherRecipient>();
}
