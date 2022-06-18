
namespace ValuedTime.App.Tests.Tests.CurrentDayServiceTests;

public class AvailableActivitiesTests : CurrentDayServiceTestFixture
{
    [Fact]
    public async Task NoExistingActivity()
    {
        await SUT.InitializeAsync(January1);

        await Verify(new { ActivityStore, ActivityLogStore, SUT.AvailableActivities });
    }

    [Fact]
    public async Task WithData()
    {
        await SUT.InitializeAsync(January1);

        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = January1_0600
        });
        await SUT.StopCurrentActivity(January1_0615);

        await Verify(new { ActivityStore, ActivityLogStore, SUT.AvailableActivities });
    }
}
