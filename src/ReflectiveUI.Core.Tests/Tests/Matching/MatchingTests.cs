using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectiveUI.Core.Tests.Tests.TreeTransform;

[UsesVerify]
public class MatchingTests
{
    // IInteractNodeProjection<InteractNode.Object>
    // Takes: ProjectionContext
    // Router uses the path of the node in the reflected graph
    // new BlazorReflector()
    // <PageType>(nodeMatcher => nodeMatcher.IsObject((o, n) => o is TestObject).
    // CreatePageRoute<PageType>(matcher => matcher.MatchObject<TestObject>().
    // ShowComponentWhen<ComponentType>(match => p.MemberType == typeof(string))

    [Fact]
    public void MatchesObject()
    {
        
    }
}
