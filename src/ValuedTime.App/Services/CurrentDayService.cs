using ValuedTime.Domain.Abstractions;
using ValuedTime.Domain.Aggregates;
using ValuedTime.Domain.Specs;
using Dto = ValuedTime.App.Models.Dto;

namespace ValuedTime.App.Services;

public class CurrentDayService
{
    private readonly IRepository<Activity> _activityRepository;
    private readonly IRepository<LifeValue> _lifeValueRepository;
    private readonly IRepository<ActivityLog> _activityLogRepository;
    private readonly IGuidFactory _guidFactory;

    private bool _initialized;

    public CurrentDayService(
        IRepository<Activity> activityRepository,
        IRepository<LifeValue> lifeValueRepository,
        IRepository<ActivityLog> activityLogRepository,
        IGuidFactory guidFactory)
    {
        _activityRepository = activityRepository;
        _lifeValueRepository = lifeValueRepository;
        _activityLogRepository = activityLogRepository;
        _guidFactory = guidFactory;
    }

    public event EventHandler<Dto.ActivityLog>? ActivityLogChanged;
    public Dto.ActivityLog? CurrentActivityLog { get; private set; }
    public List<Dto.AvailableActivity> AvailableActivities { get; private set; } = new();

    private ActivityLog? _day;

    public async Task InitializeAsync(DateOnly date)
    {
        var log = await _activityLogRepository.GetBySpecAsync(new ActivityLogByDateSpec(date));
        if (log is null)
        {
            log = new ActivityLog(new ActivityLogId(_guidFactory.NewGuid()), date);
            await _activityLogRepository.AddAsync(log);
            await _activityLogRepository.SaveChangesAsync();
        }

        _initialized = true;
        await SetActivityLogAsync(log);
        await RefreshAvailableActivities();
        Console.WriteLine($"Current day service loaded, {CurrentActivityLog?.TimeEntries.Count} Entries");
    }

    public async Task StartActivity(Dto.StartActivityCommand command)
    {
        GuardIsInitialized();

        var activity = (await _activityRepository.GetByIdAsync(new ActivityId(command.ActivityId)))
            ?? throw new InvalidOperationException("Missing activity");

        _day!.StartActivity(
            new InProgressActivity(
                activity.Id, 
                command.StartTime, 
                command.Estimate, 
                command.LifeValueIds.Select(id => new LifeValueId(id)).ToList()));
        await _activityLogRepository.UpdateAsync(_day!);
        await _activityLogRepository.SaveChangesAsync();

        await SetActivityLogAsync(_day);
    }

    public async Task StartNewActivity(Dto.StartNewActivityCommand command)
    {
        GuardIsInitialized();

        DateTime startTime = command.StartTime ?? throw new NullReferenceException(nameof(command.StartTime));

        var activity = await _activityRepository.AddAsync(new Activity(
            new ActivityId(_guidFactory.NewGuid()),
            command.ActivityName ?? throw new NullReferenceException(nameof(command.ActivityName)),
            startTime,
            ActivityStatus.Incomplete,
            completedTime: null,
            command.LifeValueIds));

        await _activityRepository.SaveChangesAsync();

        _day!.StartActivity(
            new InProgressActivity(
                activity.Id,
                startTime,
                command.Estimate,
                command.LifeValueIds.Select(id => new LifeValueId(id)).ToList()));
        await _activityLogRepository.UpdateAsync(_day!);
        await _activityLogRepository.SaveChangesAsync();

        await SetActivityLogAsync(_day);
    }

    public Task<DateTime> GetMinimumStartTimeAsync(DateTime now)
    {
        GuardIsInitialized();

        var minTime = _day!.Date.ToDateTime(TimeOnly.MinValue);

        var latestEntry = _day.TimeEntries.MaxBy(te => te.StartTime);
        if (latestEntry is not null)
        {
            if ((now - latestEntry.StartTime).TotalMinutes < 5)
                minTime = now;
            else
                minTime = latestEntry.StartTime + TimeSpan.FromMinutes(5);
        }

        return Task.FromResult(minTime);
    }

    public async Task StopCurrentActivity(DateTime endTime)
    {
        GuardIsInitialized();

        if (_day!.CurrentActivity is null)
            return;

        _day!.StopCurrentActivity(endTime);
        await _activityLogRepository.UpdateAsync(_day!);
        await _activityLogRepository.SaveChangesAsync();

        await SetActivityLogAsync(_day);
        await RefreshAvailableActivities();
    }

    public async Task DeleteTimeEntry(Guid timeEntryId)
    {
        GuardIsInitialized();

        _day!.RemoveTimeEntry(new TimeEntryId(timeEntryId));
        await _activityLogRepository.UpdateAsync(_day!);
        await _activityLogRepository.SaveChangesAsync();

        await SetActivityLogAsync(_day);
        await RefreshAvailableActivities();
    }

    private async Task RefreshAvailableActivities()
    {
        GuardIsInitialized();

        List<Dto.AvailableActivity> available = new();
        var activityIds = _day!.TimeEntries
            .Select(te => te.ActivityId)
            .Distinct();

        var lifeValuesById = (await _lifeValueRepository.ListAsync()).ToDictionary(lv => lv.Id.Value);

        foreach (var activityId in activityIds)
        {
            var activity = (await _activityRepository.GetByIdAsync(activityId))
                ?? throw new InvalidOperationException("Missing activity");

            available.Add(
                new Dto.AvailableActivity()
                {
                    ActivityId = activity.Id.Value,
                    Name = activity.Name,
                    ExpectedLifeValues = activity.ExpectedLifeValueIds
                        .Select(id => lifeValuesById[id])
                        .Select(v => new Dto.ExpectedLifeValue(v.Id.Value, v.Name))
                        .ToList()
                });
        }
        AvailableActivities = available;
    }

    private async Task SetActivityLogAsync(ActivityLog log)
    {
        _day = log;

        var lifeValuesById = (await _lifeValueRepository.ListAsync())
            .ToDictionary(lv => lv.Id.Value, lv => new Dto.LifeValue(lv.Id.Value, lv.Name));

        List<Dto.TimeEntry> timeEntries = new List<Dto.TimeEntry>(log.TimeEntries.Count);
        foreach (var te in log.TimeEntries)
        {
            var activity = (await _activityRepository.GetByIdAsync(te.ActivityId))
                ?? throw new InvalidOperationException("Missing activity");

            timeEntries.Add(
                new Dto.TimeEntry(
                    te.Id.Value,
                    te.ActivityId.Value,
                    activity.Name,
                    te.StartTime,
                    te.EndTime,
                    te.OriginalEstimate,
                    te.Duration,
                    te.LifeValueAllocations.Select(lv => lifeValuesById[lv.LifeValueId.Value]).ToList()));
        }

        Dto.InProgressActivity? currentActivity = null;
        if (log.CurrentActivity is not null)
        {
            var activity = (await _activityRepository.GetByIdAsync(log.CurrentActivity.ActivityId))
                ?? throw new InvalidOperationException("Missing activity");

            currentActivity = new Dto.InProgressActivity()
            {
                ActivityName = activity.Name,
                ActivityId = activity.Id.Value,
                StartTime = log.CurrentActivity!.StartTime,
                Estimate = log.CurrentActivity!.Estimate,
                LifeValues = log.CurrentActivity!.LifeValueIds.Select(id => lifeValuesById[id.Value]).ToList()
            };
        }

        var dto = new Dto.ActivityLog()
            {
                TimeEntries = timeEntries,
                CurrentActivity = currentActivity,
            };
        CurrentActivityLog = dto;
        ActivityLogChanged?.Invoke(this, dto);
    }

    private void GuardIsInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException("Service not initialized");
    }
}
