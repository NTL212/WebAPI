using System;
using System.Collections.Generic;

namespace ProductAPI.Models;

public partial class VoucherCampaign
{
    public int CampaignId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? TargetAudience { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<VoucherAssignment> VoucherAssignments { get; set; } = new List<VoucherAssignment>();
}
