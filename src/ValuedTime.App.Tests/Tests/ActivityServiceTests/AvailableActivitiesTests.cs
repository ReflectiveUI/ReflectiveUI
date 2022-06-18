
using ValuedTime.App.Tests.TestData;

namespace ValuedTime.App.Tests.Tests.ActivityServiceTests;

[UsesVerify]
public class AvailableActivitiesTests
{
    [Fact]
    public async Task WithNoData()
    {
        var store = new FakeRepositoryStore<Activity>();
        var repo = new InMemoryRepositoryCache<Activity, ActivityId>(store);
        var lifeValueStore = new FakeRepositoryStore<LifeValue>();
        var lifeValueRepo = new InMemoryRepositoryCache<LifeValue, LifeValueId>(lifeValueStore);
        var sut = new ActivityService(repo, lifeValueRepo, new DefaultGuidFactory());

        var activity = await sut.GetActivityAsync(Guid.NewGuid());
        await Verifier.Verify(activity);
    }
    
    [Fact]
    public async Task WithData()
    {
        var store = new FakeRepositoryStore<Activity>();
        Guid activityId = Guid.NewGuid();
        await store.StoreAll(new List<Activity>()
        {
            new Activity(new ActivityId(activityId),
                "Test Activity",
                TestDates.January1_0600,
                ActivityStatus.Incomplete,
                null,
                new())
        });
        var repo = new InMemoryRepositoryCache<Activity, ActivityId>(store);
        var lifeValueStore = new FakeRepositoryStore<LifeValue>();
        var lifeValueRepo = new InMemoryRepositoryCache<LifeValue, LifeValueId>(lifeValueStore);
        var sut = new ActivityService(repo, lifeValueRepo, new DefaultGuidFactory());

        var activity = await sut.GetActivityAsync(activityId);
        await Verifier.Verify(activity);
    }
}
