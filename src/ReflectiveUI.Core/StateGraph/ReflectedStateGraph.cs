using Microsoft.Extensions.Logging;
using ReflectiveUI.Core.ObjectGraph;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(HotReloadManager))]
namespace ReflectiveUI.Core.ObjectGraph;

internal static class HotReloadManager
{
    public static event EventHandler<Type[]?>? ApplicationShouldUpdate;
    public static void ClearCache(Type[]? types)
    {
        Console.WriteLine("ClearCache");
        ApplicationShouldUpdate?.Invoke(null, types);
    }

    public static void UpdateApplication(Type[]? types)
    {
        Console.WriteLine("Updating");
        ApplicationShouldUpdate?.Invoke(null, types);
    }
}

public class ReflectedStateGraph<T> : IReflectedStateGraph where T : notnull
{
    private readonly object _locker = new();
    private readonly T _rootInstance;
    private readonly ReflectedStateGraphOptions _settings;
    private readonly ILogger<ReflectedStateGraph<T>>? _logger;
    private readonly List<string> _traversalNamespaces = new();
    private volatile IInteractNode? _root;
    private readonly NodeContext _nodeContext;

    private Dictionary<string, IInteractNode> _nodeCache = new();

    public event EventHandler? AppUpdated;

    public T RootInstance => _rootInstance;

    public IInteractNode? RootInteractNode
    {
        get
        {
            EnsureRoot();

            return _root;
        }
    }

    private void EnsureRoot()
    {
        lock (_locker)
        {
            if (_root == null)
                Reload();
        }
    }

    public bool TryGetNode(string path, [MaybeNullWhen(false)] out IInteractNode node)
    {
        EnsureRoot();
        return _nodeCache.TryGetValue(path, out node);
    }

    public ReflectedStateGraph(T root, ReflectedStateGraphOptions? settings = null, ILogger<ReflectedStateGraph<T>>? logger = null)
    {
        _nodeContext = new(NodeUpdated);
        _rootInstance = root;
        _settings = settings ?? new();
        _logger = logger;
        _traversalNamespaces.Add(root?.GetType().Namespace!);
        _traversalNamespaces.AddRange(_settings.AdditionalNamespaces);
        HotReloadManager.ApplicationShouldUpdate += (sender, args) =>
        {
            Reload();
        };
    }

    private void NodeUpdated(IInteractNode interactNode)
    {
        // For now reload the root node until performance deems otherwise.
        Reload();
    }

    public void Reload()
    {
        IInteractNode? root = _root;
        if (root is null)
            root = new InteractNode.Object(_nodeContext, null, typeof(T), () => _rootInstance);

        var newRoot = ReloadNodeChildren(root);

        lock (_locker)
            _root = newRoot;

        AppUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void Reload(IInteractNode reloadRoot)
    {
        ReloadNodeChildren(reloadRoot);

        AppUpdated?.Invoke(this, EventArgs.Empty);
    }

    private List<IInteractNode> GetPathNodes(IInteractNode node)
    {
        var pathNodes = new List<IInteractNode>();
        var currentNode = node;
        while (currentNode is not null)
        {
            pathNodes.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        pathNodes.Reverse();
        return pathNodes;
    }

    private string GetNodePath(IInteractNode node, string basePath = "")
    {
        return basePath + "/" + string.Join(
            '/',
            GetPathNodes(node)
                .Select(n => $"{n.GetType().Name}.{n.Identifier}"));
    }

    private IInteractNode ReloadNodeChildren(IInteractNode reloadRoot)
    {
        // Reload needs to explicitly preserve any ephemeral values otherwise it will rebuild its children based on the objects in the graph directly.

        // Get the node's path before it is orphaned
        var basePath = "";
        if (reloadRoot.Parent is not null)
            basePath = GetNodePath(reloadRoot.Parent);

        // Clone a detached node with no parent so that we can update just the subtree without any side effects.
        // It remains to be seen if this causes any issues with nodes that call up to their parents.
        IMutableNode clonedRoot = ((IMutableNode)reloadRoot).Clone(null);

        var traversalStack = new Stack<IMutableNode>(new[] { clonedRoot });

        while (traversalStack.Any())
        {
            var node = traversalStack.Pop();
            var path = GetNodePath((IInteractNode)node, basePath);
            node.Path = path;

            if (node is InteractNode.Object objNode)
            {
                objNode.Children = CreateMemberNodes(objNode, _settings);
            }
            else if (node is InteractNode.Value valNode)
            {
                // Values have no children currently
            }
            else if (node is InteractNode.Enumerable enumerableNode)
            {
                var typeTemplateNode = new InteractNode.TypeInfo(_nodeContext, enumerableNode, enumerableNode.Type);
                var itemsNode = new InteractNode.EnumerableItems(_nodeContext, enumerableNode);

                enumerableNode.Children = ImmutableArray.Create(new IInteractNode[] { typeTemplateNode, itemsNode });
            }
            else if (node is InteractNode.EnumerableItems itemNode)
            {
                var items = itemNode.Parent!.CurrentItems;
                if (items is not null)
                {
                    itemNode.Children = ImmutableArray.Create(
                        items
                            .Select(obj => CreateInstanceNode(
                                itemNode, itemNode.Type, () => obj, null))
                            .Where(obj => obj is not null)
                            .Cast<IInstanceNode>()
                            .ToArray());
                }
            }
            else if (node is InteractNode.TypeInfo typeTemplateNode)
            {
                typeTemplateNode.Children = ImmutableArray.Create(new[]
                {
                    CreateInstanceNode(
                        typeTemplateNode, typeTemplateNode.Type, () => null, null)
                });
            }
            else if (node is InteractNode.Property propNode)
            {
                Action<object?>? instanceSetter = null;
                if (propNode.IsEditable)
                    instanceSetter = o => propNode.CurrentInstance = o;

                propNode.Children = ImmutableArray.Create(new[]
                {
                    CreateInstanceNode(
                        propNode, propNode.PropertyInfo.PropertyType, () => propNode.CurrentInstance, instanceSetter)
                });
            }
            else if (node is InteractNode.Method methodNode)
            {
                methodNode.Children = ImmutableArray.Create(
                    new IInteractNode[]
                    {
                        new InteractNode.InvokeableMethod(
                            _nodeContext,
                            methodNode,
                            methodNode.MethodInfo,
                            methodNode.DisplayName,
                            () => methodNode.Parent?.CurrentInstance)
                    });
            }
            else if (node is IInvokableNode invokeableNode)
            {
                // Try to preserve any existing result
                IInteractNode? cacheNode = null;
                _nodeCache.TryGetValue(path, out cacheNode);
                InteractNode.ReturnValue? lastResult = (cacheNode as IInvokableNode)?.LastResultNode;

                var newLastResult = new InteractNode.ReturnValue(_nodeContext, invokeableNode)
                {
                    CalledAtTime = lastResult?.CalledAtTime,
                    CurrentInstance = lastResult?.CurrentInstance
                };

                var parameterList = new InteractNode.ParameterList(_nodeContext, invokeableNode);

                ((IMutableNode)invokeableNode).Children =
                    ImmutableArray.Create(new IInteractNode[] { parameterList, newLastResult });
            }
            else if (node is InteractNode.ParameterList paramNode)
            {
                var children = paramNode.Parent!.GetParameters()
                    .Select(p => new InteractNode.Parameter(_nodeContext, paramNode, p.Name ?? "", p.Type))
                    .ToImmutableArray();

                paramNode.Children = children;
            }
            else if (node is InteractNode.ReturnValue resultNode)
            {
                resultNode.Children = ImmutableArray.Create(new[]
                {
                    CreateInstanceNode(resultNode, resultNode.Type, () => resultNode.CurrentInstance, null)
                });
            }

            _nodeCache[path] = (IInteractNode)node;

            foreach (var child in node.Children) { traversalStack.Push((IMutableNode)child!); }
        }

        // Reattach the node's children
        // the lock may not be strictly necessary here
        lock (_locker)
        {
            if (reloadRoot.Parent is not null)
            {
                clonedRoot.Parent = (IMutableNode)reloadRoot.Parent;
                ((IMutableNode)reloadRoot.Parent).Children = reloadRoot.Parent.Children
                    .Select(c =>
                    {
                        if (c == reloadRoot)
                            return (IInteractNode)clonedRoot;
                        return c;
                    })
                    .ToImmutableArray();
            }

            return (IInteractNode)clonedRoot;
        }
    }

    private IInstanceNode CreateInstanceNode(ITypedNode parent, Type type, Func<object?> instanceAccessor, Action<object?>? instanceSetter)
    {
        // Add instance node based on type
        if (type.IsAssignableTo(typeof(Delegate)))
        {
            return new InteractNode.InvokeableDelegate(_nodeContext, parent, parent.DisplayName, type, instanceAccessor);
        }
        else if (type.IsGenericType && type.IsAssignableTo(typeof(IEnumerable)))
        {
            type = type.GetGenericArguments()[0];
            return new InteractNode.Enumerable(_nodeContext, parent, type, instanceAccessor);
        }
        else if (_traversalNamespaces.Any(ns => type.Namespace! == ns))
        {
            return new InteractNode.Object(_nodeContext, parent, type, instanceAccessor);
        }
        else
        {
            if (_logger?.IsEnabled(LogLevel.Trace) ?? false)
                _logger.LogTrace(
                    "Interpreting {type} as a value because its namespace is not registered with the app host.", type);

            return new InteractNode.Value(_nodeContext, parent, type, instanceAccessor, instanceSetter);
        }
    }

    private readonly HashSet<string> _ignoredMemberNames = new HashSet<string>(new[]
    {
        nameof(object.ToString),
        nameof(object.GetHashCode),
        nameof(object.GetType),
        nameof(object.Equals),
        nameof(ICloneable.Clone),
        "Deconstruct",
        "<Clone>$",
    });

    private ImmutableArray<IMemberNode> CreateMemberNodes(IInstanceNode node, ReflectedStateGraphOptions settings)
    {
        var children = node.Type.GetMembers()
            .Select(m => CreateMemberNode(node, m))
            .Where(n => n is not null)
            .Cast<IMemberNode>()
            .ToList();

        // Cannot suppress null values because type info nodes describe a tree with no target instance
        if (AnyParentIs<InteractNode.TypeInfo>(node))
            settings = settings with { SuppressNullProperties = false };

        UpdateSuppression(children, settings);
        return children.ToImmutableArray();
    }

    private bool AnyParentIs<TCheck>(IInteractNode node)
    {
        var nextParent = node.Parent;
        while (nextParent is not null)
        {
            if (nextParent is TCheck)
                return true;
            nextParent = nextParent.Parent;
        }
        return false;
    }

    private IInteractNode? CreateMemberNode(IInstanceNode parent, MemberInfo m)
    {
        var displayAttribute = m.GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute?.GetAutoGenerateField() == false
            || _ignoredMemberNames.Contains(m.Name))
        {
            return null;
        }

        if (m is PropertyInfo p)
        {
            return new InteractNode.Property(_nodeContext, parent, p.PropertyType, p);
        }
        else if (m is MethodInfo mi)
        {
            if (mi.IsSpecialName || mi.DeclaringType == typeof(object))
                return null;

            InteractNode.Method methodNode = new InteractNode.Method(_nodeContext, parent, mi);

            return methodNode;
        }

        return null;
    }

    private void UpdateSuppression(List<IMemberNode> nodes, ReflectedStateGraphOptions settings)
    {
        if (settings.SupressIdProperties)
        {
            var suppressedProperties = nodes
                .Where(c => c switch
                {
                    InteractNode.Property m => m.PropertyInfo.Name.ToLowerInvariant().EndsWith("id"),
                    _ => false
                })
                .ToList();

            suppressedProperties.ForEach(sm =>
                ((IMutableNode)sm).Suppression =
                    new NodeSuppression(true, $"Property ends with 'id' and {nameof(settings.SupressIdProperties)} is true."));
        }

        if (settings.SuppressNullProperties)
        {
            var suppressedProperties = nodes
                .Where(c => c switch
                {
                    InteractNode.Property m => m.CurrentInstance is null,
                    _ => false
                })
                .ToList();

            suppressedProperties.ForEach(sm =>
                ((IMutableNode)sm).Suppression =
                    new NodeSuppression(true, $"Property value is null and {nameof(settings.SuppressNullProperties)} is true."));
        }

        if (settings.SuppressMethodsUsingCanPropertyPrefix)
        {
            var matchedProps = new List<InteractNode.Property>();
            var suppressedMethods = nodes
                .Where(c =>
                {
                    if (c is InteractNode.Method m)
                    {
                        var matchName = $"Can{m.MethodInfo.Name}";
                        var matchProp = nodes
                            .Where(c2 => c2 is InteractNode.Property)
                            .Cast<InteractNode.Property>()
                            .FirstOrDefault(pv => pv.PropertyInfo.Name == matchName);
                        if (matchProp is not null)
                        {
                            matchedProps.Add(matchProp);
                            return matchProp?.CurrentInstance as bool? == false;
                        }
                    }
                    return false;
                })
                .Cast<InteractNode.Method>()
                .ToList();

            matchedProps.ForEach(sm =>
                sm.Suppression =
                    new NodeSuppression(true, $"{nameof(settings.SuppressMethodsUsingCanPropertyPrefix)} is enabled."));

            suppressedMethods.ForEach(sm =>
                sm.Suppression =
                    new NodeSuppression(true, $"Can{sm.MethodInfo.Name} is false."));
        }
    }

    private void PrintTree(IInteractNode node)
    {
        TraverseNodesDepthFirst(node, dn =>
        {
            var s = $"{new string(Enumerable.Repeat('-', dn.Depth).ToArray())}{dn.Node.GetType().Name} {dn.Node.DisplayName}";
            Console.WriteLine(s);
        });
    }

    private void TraverseNodesDepthFirst(IInteractNode rootNode, Action<(int Depth, IInteractNode Node)> visitor)
    {
        var appNodes = new Stack<(int Depth, IInteractNode Node)>(new[] { (0, rootNode) });

        while (appNodes.Any())
        {
            var depthAndNode = appNodes.Pop();

            visitor(depthAndNode);

            var (depth, node) = depthAndNode;
            ++depth;

            foreach (var n in node.Children)
            {
                appNodes.Push((depth, n));
            }
        }
    }
}
