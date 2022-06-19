
using System.Reflection;

namespace ValuedTime.Quick.Host;

public interface IMemberNode : IInteractNode
{
    MemberInfo MemberInfo { get; }
}
