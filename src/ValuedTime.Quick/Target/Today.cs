using ValuedTime.App.Models.Dto;
using ValuedTime.App.Services;
using ValuedTime.Quick.Target.Commands;

namespace ValuedTime.Quick.Target;

public class Today
{
    private readonly CurrentDayService _currentDayService;

    public Today(CurrentDayService currentDayService)
    {
        _currentDayService = currentDayService;
        UpdateState();
    }

    private void UpdateState()
    {
        if (_currentDayService.CurrentActivityLog?.CurrentActivity is null)
        {
            StartNewActivity = new StartActivityPrompt(
                onSave: async s =>
                {
                    await _currentDayService.StartNewActivity(new StartNewActivityCommand()
                    {
                        ActivityName = s.Name,
                        Estimate = s.Estimate,
                        StartTime = DateTime.Now,
                    });
                    UpdateState();
                });
            StopCurrentActivity = null;
        }
        else
        {
            StartNewActivity = null;
            StopCurrentActivity = async () =>
            {
                await _currentDayService.StopCurrentActivity(DateTime.Now);
                UpdateState();
            };
        }
    }

    internal Task Init()
    {
        return _currentDayService.InitializeAsync(DateOnly.FromDateTime(DateTime.Now));
    }

    public StartActivityPrompt? StartNewActivity { get; private set; }
    public Func<Task>? StopCurrentActivity { get; private set; }
    public InProgressActivity? CurrentActivity => _currentDayService.CurrentActivityLog?.CurrentActivity;

    public List<TimeEntry> Entries => _currentDayService.CurrentActivityLog?.TimeEntries ?? new();
}
