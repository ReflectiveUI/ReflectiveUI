using Microsoft.AspNetCore.Components;
using ReflectiveUI.Core.ObjectGraph.Nodes;

namespace ReflectiveUI.Blazor;

public record RenderRule(Func<IInteractNode, bool> NodeMatchPredicate, RenderFragment<IInteractNode> Fragment);

public record RouteRule(Func<IInteractNode, bool> NodeMatchPredicate);