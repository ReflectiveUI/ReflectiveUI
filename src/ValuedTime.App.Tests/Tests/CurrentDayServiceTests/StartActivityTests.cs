
namespace ValuedTime.App.Tests.Tests.CurrentDayServiceTests;

[UsesVerify]
public class StartActivityTests : CurrentDayServiceTestFixture
{
    [Fact]
    public async Task WithEstimate()
    {
        await SUT.InitializeAsync(January1);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = January1_0600,
            Estimate = TimeSpan.FromMinutes(45)
        });
        var activityId = SUT.CurrentActivityLog!.CurrentActivity!.ActivityId;
        await SUT.StopCurrentActivity(January1_0615);

        await SUT.StartActivity(new Models.Dto.StartActivityCommand(activityId, January1_0630, TimeSpan.FromMinutes(45), new()));

        await Verify(new { ActivityStore, ActivityLogStore, SUT.CurrentActivityLog });
    }
}
