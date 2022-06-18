using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.App.Tests.Tests.LifeValueServiceTests;

[UsesVerify]
public class AddLifeValueTests
{
    [Fact]
    public async Task BasicValues()
    {
        var lifeValueStore = new FakeRepositoryStore<LifeValue>();
        var lifeValueRepo = new InMemoryRepositoryCache<LifeValue, LifeValueId>(lifeValueStore);
        var sut = new LifeValueService(lifeValueRepo, new DefaultGuidFactory());

        var lifeValue = await sut.AddLifeValue(new Models.Dto.CreateLifeValueCommand("Family"));
        await Verifier.Verify(new
        {
            lifeValueStore,
            lifeValue
        });
    }
}
