
namespace ValuedTime.App.Tests.Tests.CurrentDayServiceTests;

[UsesVerify]
public class StopActivityTests : CurrentDayServiceTestFixture
{
    [Fact]
    public async Task NoExistingActivity()
    {
        await SUT.InitializeAsync(January1);
        await SUT.StopCurrentActivity(January1_0600);

        await Verify(new
        {
            ActivityStore,
            ActivityLogStore,
            SUT.CurrentActivityLog
        });
    }

    [Fact]
    public async Task ExistingInProgressActivity()
    {
        await SUT.InitializeAsync(January1);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = January1_0600
        });

        await SUT.StopCurrentActivity(January1_0630);

        await Verify(new
        {
            ActivityStore,
            ActivityLogStore,
            SUT.CurrentActivityLog
        });
    }

    [Fact]
    public async Task StopExisitingActivity()
    {
        await SUT.InitializeAsync(January1);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = January1_0600
        });

        var activityId = SUT.CurrentActivityLog!.CurrentActivity!.ActivityId;

        await SUT.StopCurrentActivity(January1_0630);

        await SUT.StartActivity(new(activityId, January1_0630, null, new()));
        await SUT.StopCurrentActivity(January1_0630);

        await Verify(new
        {
            ActivityStore,
            ActivityLogStore,
            SUT.CurrentActivityLog
        });
    }
}
