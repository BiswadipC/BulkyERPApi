using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class User
{
    public int IdNo { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string IsAdmin { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public virtual ICollection<ModulePolicyMapping> ModulePolicyMappings { get; set; } = new List<ModulePolicyMapping>();
}
