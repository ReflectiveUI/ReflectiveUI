using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuickApp.Blazor;
using QuickApp.Blazor.Infrastructure;
using ValuedTime.App.Repository;
using ValuedTime.App.Services;
using ValuedTime.App.Utilities;
using ValuedTime.Domain.Abstractions;
using ValuedTime.Domain.Aggregates;
using ValuedTime.Quick.Host;
using ValuedTime.Quick.Target;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

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
builder.Services.AddScoped<DataSeedService>();
builder.Services.AddScoped<ValuedTimeApp>();
builder.Services.AddScoped<Today>();
builder.Services.AddScoped<LifeValues>();
builder.Services.AddScoped<Func<Today>>(p => () => p.GetRequiredService<Today>());
builder.Services.AddScoped<Func<ColorSelection>>(p => () => p.GetRequiredService<ColorSelection>());
builder.Services.AddScoped<History>();

builder.Services.AddScoped(c => 
    new AppHost<ValuedTimeApp>(
        root: c.GetRequiredService<ValuedTimeApp>(),
        settings: 
            new() 
            {
                AdditionalNamespaces = new() { typeof(ValuedTime.App.Models.Dto.TimeEntry).Namespace! },
            }));

var app = builder.Build();

await app.RunAsync();
