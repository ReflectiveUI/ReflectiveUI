namespace ValuedTime.App.Models.Dto;

public record TimeEntry(
    Guid Id,
    Guid ActivityId,
    string ActivityName,
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan? Estimate,
    TimeSpan Duration,
    List<LifeValue> LifeValues);
