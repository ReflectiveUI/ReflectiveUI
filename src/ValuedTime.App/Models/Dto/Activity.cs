using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValuedTime.Domain.Aggregates;

namespace ValuedTime.App.Models.Dto
{
    public record Activity(
        Guid ActivityId,
        string Name,
        DateTime CreatedTime,
        ActivityStatus ActivityStatus,
        DateTime? CompletedTime,
        List<Guid> ExpectedLifeValueIds);
}
