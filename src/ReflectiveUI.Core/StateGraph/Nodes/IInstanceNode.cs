namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public interface IInstanceNode : IInteractNode
{
    Type Type { get; }
    string? Path { get; }

    object? CurrentInstance { get; }
}
