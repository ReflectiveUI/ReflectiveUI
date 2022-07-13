using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Diagnostics.CodeAnalysis;

namespace ReflectiveUI.Core.ObjectGraph;

public interface IReflectedStateGraph
{
    IInteractNode? RootInteractNode { get; }

    event EventHandler? AppUpdated;

    void Reload();
    bool TryGetNode(string path, [MaybeNullWhen(false)] out IInteractNode node);
}