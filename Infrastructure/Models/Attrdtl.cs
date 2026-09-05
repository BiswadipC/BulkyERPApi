using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Attrdtl
{
    public int IdNo { get; set; }

    public int AttrHeadIdNo { get; set; }

    public string AttrValue { get; set; } = null!;

    public virtual AttrHead AttrHeadIdNoNavigation { get; set; } = null!;

    public virtual ICollection<ItemDtl> ItemDtls { get; set; } = new List<ItemDtl>();
}
