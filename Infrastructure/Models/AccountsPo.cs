using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class AccountsPo
{
    public int IdNo { get; set; }

    public int? AccountId { get; set; }

    public int? PartyCode { get; set; }

    public string? ModuleName { get; set; }

    public int? DocId { get; set; }

    public string? DocNo { get; set; }

    public DateOnly? DocDate { get; set; }

    public string? DrCr { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public int? IdentifierId { get; set; }
}
