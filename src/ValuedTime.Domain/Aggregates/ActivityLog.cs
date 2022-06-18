using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValuedTime.Domain.Abstractions;

namespace ValuedTime.Domain.Aggregates;

public record ActivityLog : BaseEntity<ActivityLogId>, IAggregateRoot
{
    public ActivityLog(
        ActivityLogId id,
        DateOnly date
        ) : base(id)
    {
        Date = date;
    }

    public DateOnly Date { get; }
    public InProgressActivity? CurrentActivity { get; set; }
    public List<PlannedActivity> PlannedActivities { get; init; } = new List<PlannedActivity>();
    public List<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    public void StartActivity(InProgressActivity activity)
    {
        if (CurrentActivity is not null)
            throw new InvalidOperationException("Tried to start an activity while one was already running.");

        CurrentActivity = activity;
    }
    
    public void RemoveTimeEntry(TimeEntryId id)
    {
        var timeEntry = TimeEntries.Single(te => te.Id == id);
        TimeEntries.Remove(timeEntry);
    }

    public void StopCurrentActivity(DateTime endTime)
    {
        if (CurrentActivity is null)
            return;

        // TODO: Ambient context for GUID creation?
        TimeSpan duration = endTime - CurrentActivity.StartTime;
        TimeEntries.Add(new TimeEntry(
            new TimeEntryId(Guid.NewGuid()),
            CurrentActivity.ActivityId,
            CurrentActivity.StartTime,
            endTime,
            CurrentActivity.Estimate,
            duration,
            CurrentActivity.LifeValueIds
                .Select(id => new LifeValueAllocation(id, duration))
                .ToList()));

        // Cleanup the time entries
        TimeEntries = TimeEntries.OrderBy(te => te.StartTime).ToList();
        for (int i = 0; i < TimeEntries.Count; i++)
        {
            var curr = TimeEntries[i];

            if (i < TimeEntries.Count - 1)
            {
                var next = TimeEntries[i + 1];

                if (curr.EndTime > next.StartTime)
                {
                    curr = curr with
                    {
                        EndTime = next.StartTime
                    };
                }
            }

            curr = curr with
            {
                Duration = curr.EndTime - curr.StartTime,
            };

            TimeEntries[i] = curr;
        }

        CurrentActivity = null;
    }
}

public record PlannedActivity(ActivityId ActivityId);

public record TimeEntry : BaseEntity<TimeEntryId>
{
    public TimeEntry(
        TimeEntryId id,
        ActivityId activityId,
        DateTime startTime,
        DateTime endTime,
        TimeSpan? originalEstimate,
        TimeSpan duration,
        List<LifeValueAllocation> lifeValueAllocations
        ) : base(id)
    {
        ActivityId = activityId;
        StartTime = startTime;
        EndTime = endTime;
        OriginalEstimate = originalEstimate;
        Duration = duration;
        LifeValueAllocations = lifeValueAllocations;
    }

    public ActivityId ActivityId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public TimeSpan? OriginalEstimate { get; init; }
    public TimeSpan Duration { get; init; }
    public List<LifeValueAllocation> LifeValueAllocations { get; init; } = new List<LifeValueAllocation>();
}

public record InProgressActivity(ActivityId ActivityId, DateTime StartTime, TimeSpan? Estimate, List<LifeValueId> LifeValueIds);
public record LifeValueAllocation(LifeValueId LifeValueId, TimeSpan Duration);

public record struct ActivityLogId(Guid Value);
public record struct TimeEntryId(Guid Value);

