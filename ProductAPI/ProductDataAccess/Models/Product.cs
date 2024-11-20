using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProductDataAccess.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public decimal Price { get; set; }

    public int? Stock { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public string? ImgName { get; set; }

    public virtual Category? Category { get; set; }
    [JsonIgnore]

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
