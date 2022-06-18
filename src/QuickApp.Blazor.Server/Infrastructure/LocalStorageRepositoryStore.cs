using Blazored.LocalStorage;
using ValuedTime.App.Repository;
using ValuedTime.Domain.Abstractions;

namespace QuickApp.Blazor.Server.Infrastructure;

public class LocalStorageRepositoryStore<T> : IRepositoryStore<T> where T : IAggregateRoot
{
    private readonly ILocalStorageService _localStorage;

    public LocalStorageRepositoryStore(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<T>> GetAll()
    {
        if (await _localStorage.ContainKeyAsync(typeof(T).Name))
            return await _localStorage.GetItemAsync<List<T>>(typeof(T).Name);

        return new List<T>();
    }

    public Task StoreAll(ICollection<T> entities)
    {
        return _localStorage.SetItemAsync(typeof(T).Name, entities).AsTask();
    }
}
