using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.App.Models.Dto;

public class AvailableActivity
{
    public Guid ActivityId { get; set; }
    public string? Name { get; set; }
    public List<ExpectedLifeValue> ExpectedLifeValues { get; init; } = new List<ExpectedLifeValue>();
}

public record ExpectedLifeValue(Guid LifeValueId, string Name);
