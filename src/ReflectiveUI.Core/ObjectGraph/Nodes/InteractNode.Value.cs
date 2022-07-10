using Humanizer;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record Value(
        NodeContext Context, ITypedNode? Parent, Type Type, Func<object?> InstanceAccessor, Action<object?>? InstanceSetter) 
            : InteractNode<ITypedNode, IInteractNode>(Context, Parent), ITypedNode, IInstanceNode
    {
        public override string Identifier => Type.Name;

        public bool IsEditable => InstanceSetter is not null;

        public object? CurrentInstance => InstanceAccessor();

        public override string DisplayName => Type.GetCustomAttribute<DisplayAttribute>()?.Name
            ?? Type.Name.Humanize(LetterCasing.Title);
    }
}
