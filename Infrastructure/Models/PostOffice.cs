using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PostOffice
{
    public int IdNo { get; set; }

    public string PoName { get; set; } = null!;

    public virtual ICollection<PolicyDetail> PolicyDetails { get; set; } = new List<PolicyDetail>();
}
