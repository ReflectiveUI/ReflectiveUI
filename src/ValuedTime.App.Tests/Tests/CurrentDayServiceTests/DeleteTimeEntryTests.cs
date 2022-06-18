
namespace ValuedTime.App.Tests.Tests.CurrentDayServiceTests;

public class DeleteTimeEntryTests : CurrentDayServiceTestFixture
{
    [Fact]
    public async Task DeleteTimeEntry()
    {
        await SUT.InitializeAsync(January1);
        await SUT.StartNewActivity(new Models.Dto.StartNewActivityCommand
        {
            ActivityName = "Test Start Activity",
            StartTime = January1_0600,
            Estimate = TimeSpan.FromMinutes(45)
        });
        await SUT.StopCurrentActivity(January1_0615);
        await SUT.DeleteTimeEntry(SUT.CurrentActivityLog!.TimeEntries.First().Id);

        await Verify(new { ActivityStore, ActivityLogStore, SUT.CurrentActivityLog });
    }
}
