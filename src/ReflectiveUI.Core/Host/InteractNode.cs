
using System.Collections.Immutable;

namespace ValuedTime.Quick.Host;

public abstract record InteractNode<TParent, TChild>(NodeContext Context, TParent? Parent) : IInteractNode, IMutableNode
    where TParent : IInteractNode
    where TChild : class, IInteractNode
{
    public TParent? Parent { get; internal set; } = Parent;
    public abstract string DisplayName { get; }
    public abstract string Identifier { get; }

    // Subclasses may be tempted to populate their own children, but that is not their responsibility
    // don't do that.
    public ImmutableArray<TChild> Children { get; internal set; } = ImmutableArray<TChild>.Empty;
    public virtual NodeSuppression Suppression { get; internal set; } = NodeSuppression.NoSuppression;
    NodeSuppression IMutableNode.Suppression { get => this.Suppression; set => this.Suppression = value; }
    IMutableNode? IMutableNode.Parent { get => (IMutableNode?)this.Parent; set => this.Parent = (TParent?)value; }
    ImmutableArray<IInteractNode> IMutableNode.Children { get => this.Children.CastArray<IInteractNode>(); set => this.Children = value.Cast<TChild>().ToImmutableArray(); }

    IInteractNode? IInteractNode.Parent => this.Parent;
    ImmutableArray<IInteractNode> IInteractNode.Children => this.Children.CastArray<IInteractNode>();

    IMutableNode IMutableNode.Clone(IInteractNode? parent)
    {
        return this with { Parent = (TParent?)parent };
    }
}

public record NodeSuppression(bool IsSuppressed, string? SuppressionReason)
{
    public static NodeSuppression NoSuppression { get; } = new(false, null);
}
