using Humanizer;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record Enumerable(
        NodeContext Context, ITypedNode Parent, Type Type, Func<object?> InstanceAccessor) 
            : InteractNode<ITypedNode, IInteractNode>(Context, Parent), IInstanceNode
    {
        public override string Identifier => $"Enumerable<{Type.Name}>";

        public override string DisplayName => Type.GetCustomAttribute<DisplayAttribute>()?.Name
            ?? Type.Name.Humanize(LetterCasing.Title);

        public List<object?>? CurrentItems
        {
            get
            {
                var val = InstanceAccessor();
                if (val is null)
                    return null;

                return ((IEnumerable)val).Cast<object?>().ToList();
            }
        }

        public object? CurrentInstance => InstanceAccessor();
    }
}
