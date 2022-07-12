using Humanizer;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Reflection;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public abstract partial record InteractNode
{
    public record TypeInfo(NodeContext Context, IInteractNode Parent, Type Type) 
        : InteractNode<IInteractNode, IInstanceNode>(Context, Parent), ITypedNode, IInstanceNode
    {
        public override string Identifier => Type.Name;

        public override string DisplayName => Type.GetCustomAttribute<DisplayAttribute>()?.Name
            ?? Type.Name.Humanize(LetterCasing.Title);

        public object? CurrentInstance => null;
    }
}
