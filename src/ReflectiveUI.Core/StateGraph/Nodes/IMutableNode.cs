
using System.Collections.Immutable;

namespace ReflectiveUI.Core.ObjectGraph.Nodes;

internal interface IMutableNode
{
    NodeSuppression Suppression { get; set; }
    ImmutableArray<IInteractNode> Children { get; set; }
    IMutableNode? Parent { get; set; }
    IMutableNode Clone(IInteractNode? newParent);
}
