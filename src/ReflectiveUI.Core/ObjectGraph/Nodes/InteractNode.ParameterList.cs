using Humanizer;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Collections;
using System.Reflection;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public abstract partial record InteractNode
{
    public record ParameterList(NodeContext Context, IInvokableNode Parent) 
            : InteractNode<IInvokableNode, Parameter>(Context, Parent)
    {
        public override string Identifier => "Parameters";
        public override string DisplayName => Parent!.DisplayName + " Parameters";
    }
}
