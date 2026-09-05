using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ItemGst
{
    public int IdNo { get; set; }

    public int ItemId { get; set; }

    public decimal PurCgstperc { get; set; }

    public decimal PurSgstperc { get; set; }

    public decimal PurIgstperc { get; set; }

    public decimal SalesCgstperc { get; set; }

    public decimal SalesSgstperc { get; set; }

    public decimal SalesIgstperc { get; set; }

    public virtual ItemHead Item { get; set; } = null!;
}
