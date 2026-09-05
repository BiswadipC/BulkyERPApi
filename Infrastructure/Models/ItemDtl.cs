using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ItemDtl
{
    public int Idno { get; set; }

    public int ItemId { get; set; }

    public int AttrHeadIdNo { get; set; }

    public int AttrDtlIdNo { get; set; }

    public virtual Attrdtl AttrDtlIdNoNavigation { get; set; } = null!;

    public virtual AttrHead AttrHeadIdNoNavigation { get; set; } = null!;

    public virtual ItemHead Item { get; set; } = null!;
}
