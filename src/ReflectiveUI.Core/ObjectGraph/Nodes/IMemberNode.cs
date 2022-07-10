
using System.Reflection;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public interface IMemberNode : IInteractNode
{
    MemberInfo MemberInfo { get; }
}
