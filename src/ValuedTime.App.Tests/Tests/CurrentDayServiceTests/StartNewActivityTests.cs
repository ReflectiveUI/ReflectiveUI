
namespace ValuedTime.App.Tests.Tests.CurrentDayServiceTests;

[UsesVerify]
public class StartNewActivityTests : CurrentDayServiceTestFixture
{
    [Fact]
    public async Task NoExistingActivity()
    {
        await SUT.InitializeAsync(January1);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = new DateTime(2022, 1, 1, 6, 0, 0)
        });

        await Verify(new { ActivityStore, ActivityLogStore, SUT.CurrentActivityLog });
    }

    [Fact]
    public async Task WithEstimate()
    {
        await SUT.InitializeAsync(January1);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = new DateTime(2022, 1, 1, 6, 0, 0),
            Estimate = TimeSpan.FromMinutes(45)
        });

        await Verify(new { ActivityStore, ActivityLogStore, SUT.CurrentActivityLog });
    }

    [Fact]
    public async Task SecondActivityNoOverlap()
    {
        await SUT.InitializeAsync(January1);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = January1_0600,
        });
        await SUT.StopCurrentActivity(January1_0615);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Overlapping Activity",
            StartTime = January1_0630,
        });
        await SUT.StopCurrentActivity(January1_0645);

        await Verify(new { ActivityStore, ActivityLogStore, SUT.CurrentActivityLog });
    }

    [Fact]
    public async Task SecondActivityOverlaps()
    {
        await SUT.InitializeAsync(January1);

        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = January1_0600,
        });
        await SUT.StopCurrentActivity(January1_0630);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Overlapping Activity",
            StartTime = January1_0615,
        });
        await SUT.StopCurrentActivity(January1_0645);

        await Verify(new { ActivityStore, ActivityLogStore, SUT.CurrentActivityLog });
    }
}
