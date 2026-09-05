using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PurBillHead
{
    public int BillId { get; set; }

    public string BillNo { get; set; } = null!;

    public DateOnly BillDate { get; set; }

    public int? PartyCode { get; set; }

    public int AccountId { get; set; }

    public int PurAccountId { get; set; }

    public decimal NetAmount { get; set; }

    public decimal? AccountsAdjAmount { get; set; }

    public string? Remarks { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual PartyMaster? PartyCodeNavigation { get; set; }

    public virtual Account PurAccount { get; set; } = null!;

    public virtual ICollection<PurBillDtl> PurBillDtls { get; set; } = new List<PurBillDtl>();
}
