using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public string UnitName { get; set; } = null!;
}
