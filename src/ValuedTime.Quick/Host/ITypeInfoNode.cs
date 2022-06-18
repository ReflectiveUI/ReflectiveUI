
namespace ValuedTime.Quick.Host;

// This interface may not be necessary
public interface ITypedNode : IInteractNode
{
    Type Type { get; }
}
