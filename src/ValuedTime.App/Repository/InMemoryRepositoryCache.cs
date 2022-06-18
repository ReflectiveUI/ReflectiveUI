using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ardalis.Specification;

using ValuedTime.Domain.Abstractions;
using ValuedTime.Domain.Aggregates;

namespace ValuedTime.App.Repository;

public class InMemoryRepositoryCache<T, TId> : IRepository<T>
    where T : BaseEntity<TId>, IAggregateRoot
    where TId : notnull
{
    private int _changes = 0;
    private readonly IRepositoryStore<T> _store;

    public InMemoryRepositoryCache(
        IRepositoryStore<T> store)
    {
        _store = store;
    }

    private Dictionary<TId, T>? _data;
    private Dictionary<TId, T> Data
    {
        get
        {
            if (_data is null)
                throw new InvalidOperationException("Data not loaded");

            return _data;
        }
    }

    private async Task EnsureDataLoaded()
    {
        _data = (await _store.GetAll()).ToDictionary(x => x.Id, x => x);
    }

    private async Task SaveData()
    {
        await _store.StoreAll(Data.Values);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await EnsureDataLoaded();

        Data.Add(entity.Id, entity);

        _changes++;

        return entity;
    }

    public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> GetByIdAsync<TParamId>(TParamId id, CancellationToken cancellationToken = default) where TParamId : notnull
    {
        await EnsureDataLoaded();
        if (id is not TId theId)
            throw new InvalidOperationException("Bad Id Type");

        Data.TryGetValue(theId, out T? value);
        return value;
    }

    public async Task<T?> GetBySpecAsync<TSpec>(TSpec specification, CancellationToken cancellationToken = default) where TSpec : ISingleResultSpecification, ISpecification<T>
    {
        await EnsureDataLoaded();
        return specification.Evaluate(Data.Values).SingleOrDefault();
    }

    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDataLoaded();

        return new List<T>(Data.Values);
    }

    public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        await EnsureDataLoaded();
        return specification.Evaluate(Data.Values).ToList();
    }

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await SaveData();

        var originalChangeCount = _changes;
        _changes = 0;
        return originalChangeCount;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await EnsureDataLoaded();
        if (!Data.TryGetValue(entity.Id, out T? value))
            throw new InvalidOperationException("No record to update");

        Data[entity.Id] = entity;

        _changes++;
    }
}
