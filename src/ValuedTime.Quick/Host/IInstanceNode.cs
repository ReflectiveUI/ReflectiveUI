
namespace ValuedTime.Quick.Host;

public interface IInstanceNode : IInteractNode
{
    Type Type { get; }

    object? CurrentInstance { get; }
}
