using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ValuedTime.Domain.Abstractions;

namespace ValuedTime.Domain.Aggregates;

public enum ActivityStatus
{
    Incomplete,
    Complete
}

public record Activity : BaseEntity<ActivityId>, IAggregateRoot
{
    public Activity(
        ActivityId id,
        string name,
        DateTime createdTime,
        ActivityStatus activityStatus,
        DateTime? completedTime,
        List<Guid> expectedLifeValueIds
        ) : base(id)
    {
        Name = name;
        CreatedTime = createdTime;
        ActivityStatus = activityStatus;
        CompletedTime = completedTime;
        ExpectedLifeValueIds = expectedLifeValueIds;
    }

    public string Name { get; }
    public DateTime CreatedTime { get; }
    public ActivityStatus ActivityStatus { get; }
    public DateTime? CompletedTime { get; }
    public List<Guid> ExpectedLifeValueIds { get; } = new();
}

public record struct ActivityId(Guid Value);
public record ExpectedLifeValue(LifeValueId LifeValueId);
