using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PurOrderHead
{
    public int OrderId { get; set; }

    public string OrderNo { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public int PartyCode { get; set; }

    public decimal TotalAmount { get; set; }

    public string ApprovalStatus { get; set; } = null!;

    public string? Remarks { get; set; }

    public virtual PartyMaster PartyCodeNavigation { get; set; } = null!;

    public virtual ICollection<PurBillDtl> PurBillDtls { get; set; } = new List<PurBillDtl>();

    public virtual ICollection<PurOrderDtl> PurOrderDtls { get; set; } = new List<PurOrderDtl>();
}
