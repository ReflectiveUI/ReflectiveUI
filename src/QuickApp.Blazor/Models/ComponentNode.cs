
using Microsoft.AspNetCore.Components;
using ValuedTime.Quick.Host;

namespace QuickApp.Blazor.Models;

public record ComponentNode (IInteractNode Node, List<ComponentNode> Children);
