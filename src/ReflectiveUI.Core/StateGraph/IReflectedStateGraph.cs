using ReflectiveUI.Core.ObjectGraph.Nodes;

namespace ReflectiveUI.Core.ObjectGraph;

public interface IReflectedStateGraph
{
    IInteractNode? RootInteractNode { get; }

    event EventHandler? AppUpdated;

    void Reload();
}