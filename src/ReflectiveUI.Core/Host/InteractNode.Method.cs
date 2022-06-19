using Humanizer;
using System.Reflection;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record Method(
        NodeContext Context,
        IInstanceNode Parent,
        MethodInfo MethodInfo) 
            : InteractNode<IInstanceNode, IInteractNode>(Context, Parent), IMemberNode
    {
        public override string Identifier => MethodInfo.Name;

        public override string DisplayName => MethodInfo.GetCustomAttribute<DisplayAttribute>()?.Name
            ?? MethodInfo.Name.Humanize(LetterCasing.Title);

        MemberInfo IMemberNode.MemberInfo => MethodInfo;
    }
}
