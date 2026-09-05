using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PurOrderDtl
{
    public int IdNo { get; set; }

    public int? OrderId { get; set; }

    public string? OrderNo { get; set; }

    public DateOnly? OrderDate { get; set; }

    public int ItemId { get; set; }

    public int Qty { get; set; }

    public decimal Rate { get; set; }

    public decimal Amount { get; set; }

    public int? PbQtyAdj { get; set; }

    public virtual ItemHead Item { get; set; } = null!;

    public virtual PurOrderHead? Order { get; set; }

    public virtual ICollection<PurBillDtl> PurBillDtls { get; set; } = new List<PurBillDtl>();
}
