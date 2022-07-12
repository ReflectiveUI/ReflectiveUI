using Microsoft.AspNetCore.Components;
using ReflectiveUI.Core.ObjectGraph.Nodes;

namespace ReflectiveUI.Blazor;

public enum TransformAction
{
    ShowComponent,
    RouteComponent
}

public record MatchRule(TransformAction TransformAction, Func<IInteractNode, bool> NodeMatchPredicate, Func<IInteractNode, RenderFragment> componentFactory);