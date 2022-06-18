
namespace ValuedTime.Quick.Host;

public interface IAppHost
{
    IInteractNode? RootInteractNode { get; }

    event EventHandler? AppUpdated;

    void Reload();
}