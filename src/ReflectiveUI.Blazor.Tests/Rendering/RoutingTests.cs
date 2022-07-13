
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using ReflectiveUI.Blazor.Shared;
using ReflectiveUI.Core.ObjectGraph;

namespace ReflectiveUI.Blazor.Tests.Rendering;

[UsesVerify]
public class RoutingTests
{
    [Fact]
    public Task MatchesRoot()
    {
        using var context = new TestContext();

        context.Services.AddScoped(s => new ReflectedStateGraph<TestRoot>(new TestRoot()));
        context.Services.AddScoped<IReflectedStateGraph>(s => s.GetRequiredService<ReflectedStateGraph<TestRoot>>());
        context.Services.AddScoped<BlazorReflectedStateRoutingPolicy>();

        var component = context.RenderComponent<ReflectiveUIApp>();

        return Verify(component);
    }
}

public class TestRoot
{
    public string StringProperty { get; set; } = "Test Value";
}