using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string AccountName { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Schedule { get; set; } = null!;

    public string TaxStructure { get; set; } = null!;

    public string? Add1 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Pin { get; set; }

    public string? Phone { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? AccountNo { get; set; }

    public string? Ifsccode { get; set; }

    public string? BranchCode { get; set; }

    public virtual ICollection<PurBillDtl> PurBillDtlCgstledgers { get; set; } = new List<PurBillDtl>();

    public virtual ICollection<PurBillDtl> PurBillDtlDiscountLedgers { get; set; } = new List<PurBillDtl>();

    public virtual ICollection<PurBillDtl> PurBillDtlIgstledgers { get; set; } = new List<PurBillDtl>();

    public virtual ICollection<PurBillDtl> PurBillDtlSgstledgers { get; set; } = new List<PurBillDtl>();

    public virtual ICollection<PurBillHead> PurBillHeadAccounts { get; set; } = new List<PurBillHead>();

    public virtual ICollection<PurBillHead> PurBillHeadPurAccounts { get; set; } = new List<PurBillHead>();
}
