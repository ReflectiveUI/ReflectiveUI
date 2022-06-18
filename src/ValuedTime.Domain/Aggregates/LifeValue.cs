using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValuedTime.Domain.Abstractions;

namespace ValuedTime.Domain.Aggregates;

public record LifeValue : BaseEntity<LifeValueId>, IAggregateRoot
{
    public LifeValue(
        LifeValueId id,
        string name) : base(id)
    {
        Name = name;
    }

    public string Name { get; }
}

public record struct LifeValueId(Guid Value);
