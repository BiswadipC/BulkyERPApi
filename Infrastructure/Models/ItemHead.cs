using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ItemHead
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal? ReOrderLevel { get; set; }

    public decimal? Prate { get; set; }

    public decimal? Srate { get; set; }

    public virtual ICollection<ItemDtl> ItemDtls { get; set; } = new List<ItemDtl>();

    public virtual ICollection<ItemGst> ItemGsts { get; set; } = new List<ItemGst>();

    public virtual ICollection<ItemOpStock> ItemOpStocks { get; set; } = new List<ItemOpStock>();

    public virtual ICollection<PurBillDtl> PurBillDtls { get; set; } = new List<PurBillDtl>();

    public virtual ICollection<PurOrderDtl> PurOrderDtls { get; set; } = new List<PurOrderDtl>();

    public virtual ICollection<StockDtl> StockDtls { get; set; } = new List<StockDtl>();
}
