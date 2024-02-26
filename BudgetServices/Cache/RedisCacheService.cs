using System.Text;
using StackExchange.Redis;

namespace BudgetServices.Cache;

public class RedisCacheService<T>: ICacheService<T> where T: class
{
    private readonly IDatabase _cache;
    private readonly string _prefix;

    public RedisCacheService(IConnectionMultiplexer connection)
    {
        _cache = connection.GetDatabase();
        
        Type t = typeof(T);
        _prefix = $"{t.Name}:";
    }

    public async Task<T?> GetFromCache(string id)
    {
        string? cachedVal = await GetStringFromCache(id);
        return cachedVal is not null
            ? System.Text.Json.JsonSerializer.Deserialize<T>(cachedVal)
            : null;
    }

    public async Task<string?> GetStringFromCache(string id)
    {
        return await _cache.StringGetAsync($"{_prefix}{id}");
    }

    public async Task UpdateCache(T obj, string id)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(obj);
        await _cache.StringSetAsync($"{_prefix}{id}", json);
    }

    public async Task DeleteCache(string id)
    {
        await _cache.KeyDeleteAsync($"{_prefix}{id}");
    }
}