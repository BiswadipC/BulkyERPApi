using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PolicyDetail
{
    public int IdNo { get; set; }

    public string PolicyNo { get; set; } = null!;

    public DateOnly PolicyDate { get; set; }

    public DateOnly MaturityDate { get; set; }

    public decimal? PolicyAmount { get; set; }

    public decimal? MaturityAmount { get; set; }

    public int? PoIdNo { get; set; }

    public virtual PostOffice? PoIdNoNavigation { get; set; }
}
