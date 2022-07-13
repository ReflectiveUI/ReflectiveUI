
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using ReflectiveUI.Blazor.Shared;
using ReflectiveUI.Core.ObjectGraph;

namespace ReflectiveUI.Blazor.Tests.StateTransformation;

[UsesVerify]
public class StateTransformationTests
{
    [Fact]
    public Task MatchesObject()
    {
        using var testContext = new TestContext();

        testContext.Services.AddSingleton(s =>
        {
            var t = new ReflectiveStateTransformer(new ReflectedStateGraph<TestRoot>(new TestRoot()));
            return t;
        });

        var component = testContext.RenderComponent<ReflectiveUIApp>();

        return Verify(component);
    }
}

public class TestRoot
{
    public string StringProperty { get; set; } = "Test Value";
}