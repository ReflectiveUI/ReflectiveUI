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
/// Handles routing for paths, returning a fragment to use for rendering pages.
/// </summary>
public class ReflectiveStateTransformer
{
    private readonly IReflectedStateGraph _graph;
    private readonly List<RouteRule> _routeRules = new();
    private readonly List<RenderRule> _renderRules = new();

    public IEnumerable<RouteRule> RouteRules => _routeRules;
    public IEnumerable<RenderRule> RenderRules => _renderRules;

    public ReflectiveStateTransformer(IReflectedStateGraph graph)
    {
        _graph = graph;
    }

    public void RoutePageWhen(Func<IInteractNode, bool> nodeMatchPredicate, string route, RenderFragment<IInteractNode> fragment)
    {
        _routeRules.Add(new RouteRule(nodeMatchPredicate, route, fragment));
    }

    public void ShowComponentWhen(Func<IInteractNode, bool> nodeMatchPredicate, RenderFragment<IInteractNode> fragment)
    {
        _renderRules.Add(new RenderRule(nodeMatchPredicate, fragment));
    }

    public bool IsRouteAvailable(string route)
    {
        return _routeRules.Any(r => r.Route == route);
    }

    public RenderFragment RenderRoute(string route)
    {
        // Return the matching route
        return _routeRules.First(r => r.Route == route).Fragment(_graph.RootInteractNode!);
    }
}
