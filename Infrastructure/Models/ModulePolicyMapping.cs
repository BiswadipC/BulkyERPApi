using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ModulePolicyMapping
{
    public int IdNo { get; set; }

    public string ModuleName { get; set; } = null!;

    public int UserIdNo { get; set; }

    public string PolicyName { get; set; } = null!;

    public string PermissionType { get; set; } = null!;

    public string IsAdmin { get; set; } = null!;

    public virtual User UserIdNoNavigation { get; set; } = null!;
}
