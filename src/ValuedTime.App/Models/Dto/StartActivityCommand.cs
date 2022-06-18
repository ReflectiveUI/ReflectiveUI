using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ValuedTime.Domain.Aggregates;

namespace ValuedTime.App.Models.Dto;

public record StartActivityCommand(Guid ActivityId, DateTime StartTime, TimeSpan? Estimate, List<Guid> LifeValueIds);
