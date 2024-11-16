using System.Text;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BudgetServices.Cache;

public class RedisCacheService<T>: ICacheService<T> where T: class
{
    private readonly IDatabase _cache;
    private readonly string _prefix;
    private readonly ILogger<RedisCacheService<T>> _logger;

    public RedisCacheService(IConnectionMultiplexer connection, ILogger<RedisCacheService<T>> logger)
    {
        _logger = logger;

        try
        {
            _logger.LogInformation("Connecting to redis cache");
            _cache = connection.GetDatabase();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to connect to redis cache");
            Console.WriteLine(e);
            throw;
        }
        
        Type t = typeof(T);
        _prefix = $"{t.Name}:";
        _logger.LogInformation("Initialized cache for {t.Name} with prefix `{_prefix}`", t.Name, _prefix);
    }

    public async Task<T?> GetFromCache(string id)
    {
        _logger.LogInformation("Looking for `{_prefix}` with id `{id}`", _prefix, id);
        string? cachedVal = await GetStringFromCache(id);
        if (cachedVal is null)
        {
            _logger.LogInformation("NOT FOUND `{_prefix}` with id `{id}`", _prefix, id);
            return null;
        }
        _logger.LogInformation("FOUND `{_prefix}` with id `{id}`", _prefix, id);
        return System.Text.Json.JsonSerializer.Deserialize<T>(cachedVal);
    }

    public async Task<string?> GetStringFromCache(string id)
    {
        return await _cache.StringGetAsync($"{_prefix}{id}");
    }

    public async Task UpdateCache(T obj, string id)
    {
        _logger.LogInformation("Updating cache of `{_prefix}` with id {id}", _prefix, id);
        string json = System.Text.Json.JsonSerializer.Serialize(obj);
        await _cache.StringSetAsync($"{_prefix}{id}", json);
    }

    public async Task DeleteCache(string id)
    {
        _logger.LogInformation("Deleting cache of `{_prefix}` with id {id}", _prefix, id);
        await _cache.KeyDeleteAsync($"{_prefix}{id}");
    }
}