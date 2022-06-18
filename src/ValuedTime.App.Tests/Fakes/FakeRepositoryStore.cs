using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValuedTime.App.Repository;
using ValuedTime.Domain.Abstractions;

namespace ValuedTime.App.Tests.Fakes;

public class FakeRepositoryStore<T> : IRepositoryStore<T> where T : IAggregateRoot
{
    public List<T> StoredItems { get; private set; } = new List<T>();

    public Task<List<T>> GetAll()
    {
        return Task.FromResult(StoredItems);
    }

    public Task StoreAll(ICollection<T> entities)
    {
        StoredItems = new List<T>(entities);
        return Task.CompletedTask;
    }
}
