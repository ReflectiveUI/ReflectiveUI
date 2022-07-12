using Microsoft.AspNetCore.Components;
using ReflectiveUI.Core.ObjectGraph;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectiveUI.Blazor;

/// <summary>
/// The BlazorReflector class handles routing for paths, returning Blazor components to use for rendering pages
/// It uses internal defaults to display a simple collapsible tree, additional transformation rules can be defined
/// to customize the behavior.
/// </summary>
public class BlazorReflector
{
    private List<MatchRule> _matchRules = new();
    private IReflectedStateGraph _graph;

    public BlazorReflector(IReflectedStateGraph graph)
    {
        _graph = graph;
    }

    public IEnumerable<MatchRule> MatchRules => _matchRules;

    public void RoutePageWhen(Func<IInteractNode, bool> nodeMatchPredicate, Func<IInteractNode, RenderFragment> componentFactory)
    {
        _matchRules.Add(new MatchRule(TransformAction.RouteComponent, nodeMatchPredicate, componentFactory));
    }

    public void ShowComponentWhen(Func<IInteractNode, bool> nodeMatchPredicate, Func<IInteractNode, RenderFragment> componentFactory)
    {
        _matchRules.Add(new MatchRule(TransformAction.ShowComponent, nodeMatchPredicate, componentFactory));
    }
}
