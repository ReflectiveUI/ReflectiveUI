using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectiveUI.Blazor.Models;

public enum WellKnownReason
{
    IsNull,
    Other,
}

public record SuppressionStatus(WellKnownReason? Reason, string? SuppressionReason = null)
{
    public bool IsSuppressed => Reason is not null;

    public static SuppressionStatus None { get; } = new(null, null);
}
