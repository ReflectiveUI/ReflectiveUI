using Humanizer;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Reflection;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public abstract partial record InteractNode
{
    public record Property(
        NodeContext Context, IInstanceNode Parent, Type Type, PropertyInfo PropertyInfo) 
            : InteractNode<IInstanceNode, IInstanceNode>(Context, Parent), IMemberNode, ITypedNode
    {
        public override string Identifier => PropertyInfo.Name;

        public override string DisplayName => PropertyInfo.GetCustomAttribute<DisplayAttribute>()?.Name
            ?? PropertyInfo.Name.Humanize(LetterCasing.Title);

        public bool IsEditable =>
            Parent!.CurrentInstance is not null
            && PropertyInfo.GetSetMethod() is not null; // Is public setter

        public object? CurrentInstance
        {
            get
            {
                var i = Parent!.CurrentInstance;
                if (i is null)
                    return null;

                return PropertyInfo.GetValue(i);
            }
            set
            {
                if (!IsEditable)
                    throw new InvalidOperationException("Property is not editable");

                var i = Parent!.CurrentInstance;
                if (i is null)
                    throw new InvalidOperationException("Object is null");

                PropertyInfo.SetValue(i, value);
                Context.NodeUpdated(this);
            }
        }

        public MemberInfo MemberInfo => PropertyInfo;
    }
}
