using Humanizer;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Collections;
using System.Reflection;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public abstract partial record InteractNode
{
    public record Parameter(NodeContext Context, ParameterList Parent, string DisplayName, Type Type) 
            : InteractNode<ParameterList, IInteractNode>(Context, Parent)
    {
        public override string Identifier => Type.Name;
        public override string DisplayName { get; } = DisplayName;
        public object? CurrentValue { get; set; }
    }
}
