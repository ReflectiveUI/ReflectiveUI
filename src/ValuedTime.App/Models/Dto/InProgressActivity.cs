using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ValuedTime.App.Models.Dto;

public record InProgressActivity
{
    public string? ActivityName { get; set; }
    public Guid ActivityId { get; set; }
    public DateTime StartTime { get; set; }
    public List<LifeValue> LifeValues { get; set; } = new();
    public TimeSpan? Estimate { get; set; }
}
