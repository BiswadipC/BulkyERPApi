using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PartyMaster
{
    public int PartyCode { get; set; }

    public string PartyName { get; set; } = null!;

    public string? Add1 { get; set; }

    public string? Add2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Pin { get; set; }

    public string? Mobile { get; set; }

    public string? Gstno { get; set; }

    public string? DrugLicenceNo { get; set; }

    public virtual ICollection<PurBillHead> PurBillHeads { get; set; } = new List<PurBillHead>();

    public virtual ICollection<PurOrderHead> PurOrderHeads { get; set; } = new List<PurOrderHead>();
}
