using BeatGeneratorAPI;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ReflectiveUI.Core.ObjectGraph;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ClockGenerator>();

builder.Services.AddScoped(c => 
    new ReflectedObjectGraph<ClockGenerator>(
        root: c.GetRequiredService<ClockGenerator>(),
        logger: c.GetRequiredService<ILogger<ReflectedObjectGraph<ClockGenerator>>>()));

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
