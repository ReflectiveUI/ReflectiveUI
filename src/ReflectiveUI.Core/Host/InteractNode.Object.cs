using Humanizer;
using System.Reflection;
using System.Runtime.Serialization;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record Object(NodeContext Context, ITypedNode? Parent, Type Type, Func<object?> InstanceAccessor) 
        : InteractNode<ITypedNode, IMemberNode>(Context, Parent), ITypedNode, IInstanceNode
    {
        public override string Identifier => Type.Name;

        public override string DisplayName => Type.GetCustomAttribute<DisplayAttribute>()?.Name
            ?? Type.Name.Humanize(LetterCasing.Title);

        public object? CurrentInstance => InstanceAccessor();
    }
}
