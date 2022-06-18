using ValuedTime.Domain.Abstractions;
using ValuedTime.Domain.Aggregates;
using ValuedTime.Domain.Specs;
using Dto = ValuedTime.App.Models.Dto;

namespace ValuedTime.App.Services;

public class ActivityService
{
    private readonly IRepository<Activity> _activityRepository;
    private readonly IRepository<LifeValue> _lifeValueRepository;
    private readonly IGuidFactory _guidFactory;

    public ActivityService(
        IRepository<Activity> activityRepository,
        IRepository<LifeValue> lifeValueRepository,
        IGuidFactory guidFactory)
    {
        _activityRepository = activityRepository;
        _lifeValueRepository = lifeValueRepository;
        _guidFactory = guidFactory;
    }

    public async Task<Dto.Activity?> GetActivityAsync(Guid activityId)
    {
        var activity = await _activityRepository.GetByIdAsync(new ActivityId(activityId));
        if (activity is null)
            return null;

        return new Dto.Activity(
            activity.Id.Value,
            activity.Name,
            activity.CreatedTime,
            activity.ActivityStatus,
            activity.CompletedTime,
            activity.ExpectedLifeValueIds);
    }
}
