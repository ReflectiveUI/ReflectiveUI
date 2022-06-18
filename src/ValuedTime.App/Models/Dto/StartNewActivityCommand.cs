using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ValuedTime.Domain.Aggregates;

namespace ValuedTime.App.Models.Dto;

public record StartNewActivityCommand
{
    public string? ActivityName { get; set; }
    public DateTime? StartTime { get; set; }
    public List<Guid> LifeValueIds { get; set; } = new();
    public TimeSpan? Estimate { get; set; }
}
