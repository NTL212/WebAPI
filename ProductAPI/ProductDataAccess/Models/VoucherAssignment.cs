﻿using System;
using System.Collections.Generic;

namespace ProductDataAccess.Models;

public partial class VoucherAssignment
{
    public int AssignmentId { get; set; }

    public int? VoucherId { get; set; }

    public int? CampaignId { get; set; }

    public virtual VoucherCampaign? Campaign { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
