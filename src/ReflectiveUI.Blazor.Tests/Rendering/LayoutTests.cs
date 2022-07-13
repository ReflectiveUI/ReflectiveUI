
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using ReflectiveUI.Blazor.Shared;
using ReflectiveUI.Core.ObjectGraph;

namespace ReflectiveUI.Blazor.Tests.Rendering;

[UsesVerify]
public class LayoutTests
{
    [Fact]
    public Task NodeTemplate()
    {
        using var context = new TestContext();

        context.Services.AddScoped(s => new ReflectedStateGraph<TestRoot>(new TestRoot()));
        context.Services.AddScoped<IReflectedStateGraph>(s => s.GetRequiredService<ReflectedStateGraph<TestRoot>>());
        context.Services.AddScoped<BlazorReflectedStateRoutingPolicy>();

        var component = context.RenderComponent<ReflectiveUIApp>(
             builder =>
             {
                 builder.Add(app => app.NodeTemplate, node => "<div>HI!</div>");
             });

        return Verify(component);
    }
}