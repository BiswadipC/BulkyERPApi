using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class AttrHead
{
    public int IdNo { get; set; }

    public string AttrName { get; set; } = null!;

    public virtual ICollection<Attrdtl> Attrdtls { get; set; } = new List<Attrdtl>();

    public virtual ICollection<ItemDtl> ItemDtls { get; set; } = new List<ItemDtl>();
}
