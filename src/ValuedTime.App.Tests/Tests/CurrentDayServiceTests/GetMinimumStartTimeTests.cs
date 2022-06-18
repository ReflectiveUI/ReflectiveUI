
namespace ValuedTime.App.Tests.Tests.CurrentDayServiceTests;

public class GetMinimumStartTimeTests : CurrentDayServiceTestFixture
{
    [Fact]
    public async Task NoExistingActivity()
    {
        await SUT.InitializeAsync(January1);

        DateTime now = new DateTime(2022, 1, 1, 6, 30, 0);
        var minimumStartTime = await SUT.GetMinimumStartTimeAsync(now);

        await Verify(new { now, minimumStartTime });
    }

    [Fact]
    public async Task MoreThan5MinutesFromLastStart()
    {
        await SUT.InitializeAsync(January1);

        var lastStartTime = new DateTime(2022, 1, 1, 6, 0, 0);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = lastStartTime
        });
        await SUT.StopCurrentActivity(new DateTime(2022, 1, 1, 6, 30, 0));

        DateTime now = new DateTime(2022, 1, 1, 6, 30, 0);
        var minimumStartTime = await SUT.GetMinimumStartTimeAsync(now);

        await Verify(new { lastStartTime, now, minimumStartTime });
    }

    [Fact]
    public async Task LessThan5MinutesFromLastStart()
    {
        await SUT.InitializeAsync(January1);

        var lastStartTime = new DateTime(2022, 1, 1, 6, 0, 0);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = lastStartTime
        });

        await SUT.StopCurrentActivity(new DateTime(2022, 1, 1, 6, 2, 0));

        DateTime now = new DateTime(2022, 1, 1, 6, 2, 0);
        var minimumStartTime = await SUT.GetMinimumStartTimeAsync(now);

        await Verify(new { lastStartTime, now, minimumStartTime });
    }
}
