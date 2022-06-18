using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Text.Json;
using ValuedTime.App.Repository;
using ValuedTime.App.Services;
using ValuedTime.App.Utilities;
using ValuedTime.Domain.Abstractions;
using ValuedTime.Domain.Aggregates;
using ValuedTime.WebClient;
using ValuedTime.WebClient.Infrastructure;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage(
    opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        opt.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

builder.Services.AddScoped<IGuidFactory, DefaultGuidFactory>();
builder.Services.AddScoped(typeof(IRepositoryStore<>), typeof(LocalStorageRepositoryStore<>));

builder.Services.AddScoped<IRepository<LifeValue>>(s =>
    new InMemoryRepositoryCache<LifeValue, LifeValueId>(s.GetRequiredService<IRepositoryStore<LifeValue>>()));
builder.Services.AddScoped<IRepository<Activity>>(s =>
    new InMemoryRepositoryCache<Activity, ActivityId>(s.GetRequiredService<IRepositoryStore<Activity>>()));
builder.Services.AddScoped<IRepository<ActivityLog>>(s =>
    new InMemoryRepositoryCache<ActivityLog, ActivityLogId>(s.GetRequiredService<IRepositoryStore<ActivityLog>>()));

builder.Services.AddScoped<CurrentDayService>();
builder.Services.AddScoped<ActivityService>();
builder.Services.AddScoped<LifeValueService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var activityStore = services.GetRequiredService<IRepositoryStore<Activity>>();
        if (!(await activityStore.GetAll()).Any())
        {
            var id = new ActivityLogId(Guid.NewGuid());
            var date = DateOnly.FromDateTime(DateTime.Now);
            var activities = Enumerable.Range(0, 5)
                .Select(i => new Activity(
                    new ActivityId(Guid.NewGuid()),
                    $"Activity {i}",
                    date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(i))),
                    ActivityStatus.Incomplete,
                    null,
                    new()))
                .ToList();
            await activityStore.StoreAll(activities);

            var logStore = services.GetRequiredService<IRepositoryStore<ActivityLog>>();
            await logStore.StoreAll(new List<ActivityLog>
                {
                    new(id, date)
                    {
                        TimeEntries = activities
                            .Select((a, i) => new TimeEntry(
                                new TimeEntryId(Guid.NewGuid()),
                                a.Id,
                                date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(6 + i))),
                                date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(6 + i + 0.5d))),
                                null,
                                TimeSpan.FromHours(0.5),
                                new()))
                            .ToList()
                    }
                });
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the data.");
    }
}

await app.RunAsync();
