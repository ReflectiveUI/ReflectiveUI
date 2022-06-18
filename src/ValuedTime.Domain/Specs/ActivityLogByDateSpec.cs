using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ardalis.Specification;

using ValuedTime.Domain.Aggregates;

namespace ValuedTime.Domain.Specs;

public class ActivityLogByDateSpec : Specification<ActivityLog>, ISingleResultSpecification<ActivityLog>
{
    public ActivityLogByDateSpec(DateOnly date)
    {
        Query.Where(a => a.Date == date);
    }
}
