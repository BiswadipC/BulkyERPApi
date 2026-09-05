using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class StockDtl
{
    public int IdNo { get; set; }

    public int? ItemId { get; set; }

    public string ModuleName { get; set; } = null!;

    public int DocId { get; set; }

    public string DocNo { get; set; } = null!;

    public DateOnly DocDate { get; set; }

    public int DtlRecId { get; set; }

    public int? InQty { get; set; }

    public int? OutQty { get; set; }

    public decimal? Rate { get; set; }

    public decimal? Mrp { get; set; }

    public decimal? Amount { get; set; }

    public virtual ItemHead? Item { get; set; }
}
