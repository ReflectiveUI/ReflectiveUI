
using System.Runtime.CompilerServices;
using ValuedTime.App.Utilities;

namespace ValuedTime.App.Tests.Fixtures;

[UsesVerify]
public abstract class CurrentDayServiceTestFixture
{
    protected CurrentDayService SUT { get; set; }
    protected FakeRepositoryStore<Activity> ActivityStore { get; set; }
    protected FakeRepositoryStore<ActivityLog> ActivityLogStore { get; set; }

    protected DateOnly January1 { get; }
    protected DateTime January1_0600 { get; }
    protected DateTime January1_0615 { get; }
    protected DateTime January1_0630 { get; }
    protected DateTime January1_0645 { get; }
    public FakeRepositoryStore<LifeValue> LifeValueStore { get; }

    protected CurrentDayServiceTestFixture()
    {
        January1 = new DateOnly(2022, 1, 1);
        January1_0600 = January1.ToDateTime(new TimeOnly(6, 0));
        January1_0615 = January1.ToDateTime(new TimeOnly(6, 15));
        January1_0630 = January1.ToDateTime(new TimeOnly(6, 30));
        January1_0645 = January1.ToDateTime(new TimeOnly(6, 45));

        ActivityLogStore = new FakeRepositoryStore<ActivityLog>();
        var logRepo = new InMemoryRepositoryCache<ActivityLog, ActivityLogId>(ActivityLogStore);

        ActivityStore = new FakeRepositoryStore<Activity>();
        var activityRepo = new InMemoryRepositoryCache<Activity, ActivityId>(ActivityStore);

        LifeValueStore = new FakeRepositoryStore<LifeValue>();
        var lifeValueRepo = new InMemoryRepositoryCache<LifeValue, LifeValueId>(LifeValueStore);

        SUT = new CurrentDayService(activityRepo, lifeValueRepo, logRepo, new DefaultGuidFactory());
    }

    protected VerifyTests.SettingsTask Verify<T>(T obj, [CallerFilePath] string sourceFile = "")
    {
        return Verifier.Verify(obj, sourceFile: sourceFile)
            .AddExtraSettings(s =>
            {
                s.Converters.Insert(0, new DateOnlyNewtonsoftJsonConverter());
                s.Converters.Insert(0, new TimeOnlyNewtonsoftJsonConverter());
            })
            .ModifySerialization(_ => _.DontScrubDateTimes());
    }
}
