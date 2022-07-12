
using System.Collections.Immutable;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public interface IInteractNode
{
    string Identifier { get; }
    string DisplayName { get; }
    IInteractNode? Parent { get; }
    ImmutableArray<IInteractNode> Children { get; }
    NodeSuppression Suppression { get; }
}
