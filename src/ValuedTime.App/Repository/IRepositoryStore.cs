using ValuedTime.Domain.Abstractions;

namespace ValuedTime.App.Repository;

public interface IRepositoryStore<T> where T : IAggregateRoot
{
    Task<List<T>> GetAll();
    Task StoreAll(ICollection<T> entities);
}
