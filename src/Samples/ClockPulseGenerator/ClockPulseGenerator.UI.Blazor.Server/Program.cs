using BeatGeneratorAPI;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ReflectiveUI.Blazor;
using ReflectiveUI.Core.ObjectGraph;
using ReflectiveUI.Core.ObjectGraph.Nodes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ClockGenerator>();

builder.Services.AddScoped(c => 
    new ReflectedStateGraph<ClockGenerator>(
        root: c.GetRequiredService<ClockGenerator>(),
        logger: c.GetRequiredService<ILogger<ReflectedStateGraph<ClockGenerator>>>()));
builder.Services.AddScoped<IReflectedStateGraph>(c => c.GetRequiredService<ReflectedStateGraph<ClockGenerator>>());

builder.Services.AddScoped(s =>
{
    var t = new BlazorReflectedStateRoutingPolicy(s.GetRequiredService<IReflectedStateGraph>());
    t.RoutePageWhen(n => n is InteractNode.Object);
    return t;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
