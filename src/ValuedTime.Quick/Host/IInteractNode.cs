
using System.Collections.Immutable;

namespace ValuedTime.Quick.Host;

public interface IInteractNode
{
    string Identifier { get; }
    string DisplayName { get; }
    IInteractNode? Parent { get; }
    ImmutableArray<IInteractNode> Children { get; }
    NodeSuppression Suppression { get; }
}
