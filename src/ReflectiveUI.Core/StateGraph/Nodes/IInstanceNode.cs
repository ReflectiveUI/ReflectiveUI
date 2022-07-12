namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public interface IInstanceNode : IInteractNode
{
    Type Type { get; }

    object? CurrentInstance { get; }
}
