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

    public int? RoleId { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual UserGroup? Group { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }
}
