using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PurBillDtl
{
    public int IdNo { get; set; }

    public int BillId { get; set; }

    public string BillNo { get; set; } = null!;

    public DateOnly BillDate { get; set; }

    public int ItemId { get; set; }

    public int Qty { get; set; }

    public decimal Rate { get; set; }

    public decimal Amount { get; set; }

    public int? CgstledgerId { get; set; }

    public int? SgstledgerId { get; set; }

    public int? IgstledgerId { get; set; }

    public decimal? Cgst { get; set; }

    public decimal? Sgst { get; set; }

    public decimal? Igst { get; set; }

    public int? DiscountLedgerId { get; set; }

    public decimal? DiscountPerc { get; set; }

    public decimal? DiscountValue { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? AmountAfterDiscount { get; set; }

    public int? PodtlIdNo { get; set; }

    public int? OrderId { get; set; }

    public virtual PurBillHead Bill { get; set; } = null!;

    public virtual Account? Cgstledger { get; set; }

    public virtual Account? DiscountLedger { get; set; }

    public virtual Account? Igstledger { get; set; }

    public virtual ItemHead Item { get; set; } = null!;

    public virtual PurOrderHead? Order { get; set; }

    public virtual PurOrderDtl? PodtlIdNoNavigation { get; set; }

    public virtual Account? Sgstledger { get; set; }
}
