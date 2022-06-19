using Humanizer;
using System.Collections;
using System.Reflection;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record ParameterList(NodeContext Context, IInvokableNode Parent) 
            : InteractNode<IInvokableNode, Parameter>(Context, Parent)
    {
        public override string Identifier => "Parameters";
        public override string DisplayName => Parent!.DisplayName + " Parameters";
    }
}
