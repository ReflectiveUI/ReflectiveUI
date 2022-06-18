
namespace ValuedTime.App.Tests.Tests.CurrentDayServiceTests;

[UsesVerify]
public class InitializeTests : CurrentDayServiceTestFixture
{
    [Fact]
    public async Task Get_EmptyStore()
    {
        await SUT.InitializeAsync(January1);

        await Verify(new
        {
            ActivityStore,
            ActivityLogStore,
            SUT.CurrentActivityLog
        });
    }

    [Fact]
    public async Task LoadExistingData()
    {
        var activity = new Activity(
            new ActivityId(Guid.NewGuid()),
            "Test Activity",
            January1_0600,
            ActivityStatus.Incomplete,
            null,
            new());
        await ActivityStore.StoreAll(new List<Activity> { activity });
        await ActivityLogStore.StoreAll(new List<ActivityLog>
        {
            new ActivityLog(new ActivityLogId(Guid.NewGuid()), January1)
            {
                TimeEntries = new List<TimeEntry>
                {
                    new TimeEntry(
                        new TimeEntryId(Guid.NewGuid()),
                        activity.Id,
                        January1_0600,
                        January1_0630,
                        null,
                        TimeSpan.FromHours(0.5),
                        new())
                }
            }
        });

        await SUT.InitializeAsync(January1);

        await Verify(new
        {
            ActivityStore,
            ActivityLogStore,
            SUT.CurrentActivityLog
        });
    }
}
