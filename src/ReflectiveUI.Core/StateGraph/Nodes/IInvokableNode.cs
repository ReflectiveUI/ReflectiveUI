
using System.Reflection;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public interface IInvokableNode : IInteractNode
{
    Task<object?> InvokeAsync(params object?[]? parameters);
    List<(string? Name, Type Type)> GetParameters();
    InteractNode.ReturnValue? LastResultNode { get; }
    Type ReturnType { get; }
    bool IsAvailable { get; }
}
