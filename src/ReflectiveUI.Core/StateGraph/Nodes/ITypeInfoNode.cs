namespace ReflectiveUI.Core.ObjectGraph.Nodes;

// This interface may not be necessary
public interface ITypedNode : IInteractNode
{
    Type Type { get; }
}
