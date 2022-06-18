
using System.Collections.Immutable;

namespace ValuedTime.Quick.Host;

internal interface IMutableNode
{
    NodeSuppression Suppression { get; set; }
    ImmutableArray<IInteractNode> Children { get; set; }
    IMutableNode? Parent { get; set; }
    IMutableNode Clone(IInteractNode? newParent);
}
