
using Bunit;
using ReflectiveUI.Core.ObjectGraph;

namespace ReflectiveUI.Blazor.Tests.StateTransformation;

[UsesVerify]
public class StateTransformationTests
{
    [Fact]
    public Task MatchesObject()
    {
        //var graph = new ReflectedStateGraph<TestRoot>(new TestRoot());
        //var sut = new BlazorReflector(graph);
        //sut.RoutPageWhen(n => n.Parent is null);

        using var testContext = new TestContext();
        var component = testContext.RenderComponent<TestApp>();
        return Verify("");
    }
}

public class TestRoot
{
    public string StringProperty { get; set; } = "Test Value";
}