using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.App.Tests.TestData;

public static class TestDates
{
    public static readonly DateOnly January1 = new DateOnly(2022, 1, 1);
    public static readonly DateTime January1_0600 = January1.ToDateTime(new TimeOnly(6, 0));
    public static readonly DateTime January1_0615 = January1.ToDateTime(new TimeOnly(6, 15));
    public static readonly DateTime January1_0630 = January1.ToDateTime(new TimeOnly(6, 30));
    public static readonly DateTime January1_0645 = January1.ToDateTime(new TimeOnly(6, 45));
}
