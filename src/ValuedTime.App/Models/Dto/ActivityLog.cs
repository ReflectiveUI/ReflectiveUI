using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.App.Models.Dto;

public class ActivityLog
{
    public List<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public InProgressActivity? CurrentActivity { get; set; }
}
