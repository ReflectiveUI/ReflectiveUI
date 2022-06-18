namespace ValuedTime.Domain.Abstractions;

public abstract record BaseEntity<TId>
{
    protected BaseEntity(TId id)
    {
        Id = id;
    }

    public TId Id { get; }
}
