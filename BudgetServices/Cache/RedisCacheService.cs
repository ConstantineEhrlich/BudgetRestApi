using System.Text;
using StackExchange.Redis;

namespace BudgetServices.Cache;

public class RedisCacheService<T>: ICacheService<T> where T: class
{
    private readonly IDatabase _cache;
    private readonly StringBuilder _prefix;

    public RedisCacheService(IConnectionMultiplexer connection)
    {
        _cache = connection.GetDatabase();
        
        Type t = typeof(T);
        _prefix = new(t.Name);
        _prefix.Append(':');
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
        return await _cache.StringGetAsync(_prefix.Append(id).ToString());
    }

    public async Task UpdateCache(T obj, string id)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(obj);
        await _cache.StringSetAsync(_prefix.Append(id).ToString(), json);
    }
}