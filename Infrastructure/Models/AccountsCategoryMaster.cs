using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class AccountsCategoryMaster
{
    public int IdNo { get; set; }

    public string CategoryCode { get; set; } = null!;

    public string CategoryName { get; set; } = null!;
}
