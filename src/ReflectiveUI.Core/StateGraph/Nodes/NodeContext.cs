namespace ReflectiveUI.Core.ObjectGraph.Nodes;

public class NodeContext
{
    private readonly Action<IInteractNode> nodeUpdatedCallback;

    public NodeContext(Action<IInteractNode> nodeUpdatedCallback)
    {
        this.nodeUpdatedCallback = nodeUpdatedCallback;
    }

    public void NodeUpdated(IInteractNode node)
    {
        nodeUpdatedCallback(node);
    }
}
