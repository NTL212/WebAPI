using System;
using System.Collections.Generic;

namespace ProductAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? ReceverName { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
