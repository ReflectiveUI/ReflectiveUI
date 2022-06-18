using Humanizer;
using System.Reflection;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record EnumerableItems(
        NodeContext Context, Enumerable Parent) 
            : InteractNode<Enumerable, IInstanceNode>(Context, Parent), ITypedNode
    {
        public override string Identifier => "Items";

        public override string DisplayName => Parent!.DisplayName + " Items";

        public Type Type { get; } = Parent!.Type;
    }
}
