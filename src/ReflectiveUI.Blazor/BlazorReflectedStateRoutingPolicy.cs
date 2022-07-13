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
public class BlazorReflectedStateRoutingPolicy
{
    private readonly IReflectedStateGraph _graph;
    private readonly List<RouteRule> _routeRules = new();

    public IEnumerable<RouteRule> RouteRules => _routeRules;

    public BlazorReflectedStateRoutingPolicy(IReflectedStateGraph graph)
    {
        _graph = graph;
    }

    public void RoutePageWhen(Func<IInteractNode, bool> nodeMatchPredicate)
    {
        _routeRules.Add(new RouteRule(nodeMatchPredicate));
    }

    public IInteractNode? FindRoutableNode(string route)
    {
        if (route == "/")
        {
            return _graph.RootInteractNode;
        }

        if (_graph.TryGetNode(route, out IInteractNode? node))
        {
            var canRoute = _routeRules.Any(r => r.NodeMatchPredicate(node));
            if (canRoute)
                return node;
        }

        return null;
    }
}
