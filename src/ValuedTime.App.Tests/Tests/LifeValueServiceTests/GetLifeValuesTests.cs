using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.App.Tests.Tests.LifeValueServiceTests;

[UsesVerify]
public class GetLifeValuesTests
{
    [Fact]
    public async Task OneValue()
    {
        var lifeValueStore = new FakeRepositoryStore<LifeValue>();
        var lifeValueRepo = new InMemoryRepositoryCache<LifeValue, LifeValueId>(lifeValueStore);
        var sut = new LifeValueService(lifeValueRepo, new DefaultGuidFactory());

        await sut.AddLifeValue(new Models.Dto.CreateLifeValueCommand("Family"));
        var values = await sut.GetAllLifeValues();
        await Verifier.Verify(values);
    }
}
