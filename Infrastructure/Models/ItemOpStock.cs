using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ItemOpStock
{
    public int IdNo { get; set; }

    public int? ItemId { get; set; }

    public int Qty { get; set; }

    public decimal? Rate { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly SystemOpDate { get; set; }

    public virtual ItemHead? Item { get; set; }
}
