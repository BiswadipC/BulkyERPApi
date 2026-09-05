using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class SystemVariable
{
    public string VariableName { get; set; } = null!;

    public string VariableValue { get; set; } = null!;
}
