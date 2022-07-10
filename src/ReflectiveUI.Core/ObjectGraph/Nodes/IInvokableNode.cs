
using System.Reflection;
using static ValuedTime.Quick.Host.InteractNode;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public interface IInvokableNode : IInteractNode
{
    Task<object?> InvokeAsync(params object?[]? parameters);
    List<(string? Name, Type Type)> GetParameters();
    ReturnValue? LastResultNode { get; }
    Type ReturnType { get; }
    bool IsAvailable { get; }
}
