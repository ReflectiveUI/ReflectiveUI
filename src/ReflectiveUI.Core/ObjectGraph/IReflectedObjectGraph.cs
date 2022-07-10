using ReflectiveUI.Core.ObjectGraph.Nodes;

namespace ReflectiveUI.Core.ObjectGraph;

public interface IReflectedObjectGraph
{
    IInteractNode? RootInteractNode { get; }

    event EventHandler? AppUpdated;

    void Reload();
}