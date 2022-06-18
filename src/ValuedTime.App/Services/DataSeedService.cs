using ValuedTime.App.Repository;
using ValuedTime.Domain.Abstractions;
using ValuedTime.Domain.Aggregates;

namespace ValuedTime.App.Services;

public class DataSeedService
{
    private readonly IRepositoryStore<Activity> activityStore;
    private readonly IRepositoryStore<LifeValue> lifeValueStore;
    private readonly IRepositoryStore<ActivityLog> activityLogStore;
    private readonly IGuidFactory guidFactory;

    public DataSeedService(
        IRepositoryStore<Activity> activityStore,
        IRepositoryStore<LifeValue> lifeValueStore,
        IRepositoryStore<ActivityLog> activityLogStore,
        IGuidFactory guidFactory)
    {
        this.activityStore = activityStore;
        this.lifeValueStore = lifeValueStore;
        this.activityLogStore = activityLogStore;
        this.guidFactory = guidFactory;
    }
    
    public async Task ClearAsync()
    {
        await lifeValueStore.StoreAll(new List<LifeValue>());
        await activityStore.StoreAll(new List<Activity>());
        await activityLogStore.StoreAll(new List<ActivityLog>());
    }

    public async Task ResetAsync()
    {
        await lifeValueStore.StoreAll(new List<LifeValue>());

        var id = new ActivityLogId(guidFactory.NewGuid());
        var date = DateOnly.FromDateTime(DateTime.Now);
        var activities = Enumerable.Range(0, 5)
            .Select(i => new Activity(
                new ActivityId(guidFactory.NewGuid()),
                $"Activity {i}",
                date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(i))),
                ActivityStatus.Incomplete,
                null,
                new()))
            .ToList();
        await activityStore.StoreAll(activities);

        await activityLogStore.StoreAll(new List<ActivityLog>
            {
                new(id, date)
                {
                    TimeEntries = activities
                        .Select((a, i) => new TimeEntry(
                            new TimeEntryId(guidFactory.NewGuid()),
                            a.Id,
                            date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(6 + i))),
                            date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(6 + i + 0.5d))),
                            null,
                            TimeSpan.FromHours(0.5),
                            new()))
                        .ToList()
                }
            });
    }
}
