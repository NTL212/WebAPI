using System;
using System.Collections.Generic;

namespace ProductDataAccess.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public int? GroupId { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<VoucherRecipient> VoucherRecipients { get; set; } = new List<VoucherRecipient>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
