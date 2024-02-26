namespace BudgetServices.Cache;

/// <summary>
/// Provides cache functionality
/// </summary>
/// <typeparam name="T">Entity model class</typeparam>
public interface ICacheService<T> where T : class
{
    /// <summary>
    /// Returns an instance of T by deserializing it from the cache.
    /// </summary>
    /// <param name="id">Id of the object</param>
    /// <returns>T, or Null when cached value not found</returns>
    Task<T?> GetFromCache(string id);

    /// <summary>
    /// Gets 
    /// </summary>
    /// <param name="id">Id of the object</param>
    /// <returns>String</returns>
    Task<string?> GetStringFromCache(string id);
    
    /// <summary>
    /// Stores an object in the cache
    /// </summary>
    /// <param name="obj">Object to store</param>
    /// <param name="id">Id of the object</param>
    Task UpdateCache(T obj, string id);

    /// <summary>
    /// Deletes the object from the cache
    /// </summary>
    /// <param name="id">Id of the object</param>
    /// <returns></returns>
    Task DeleteCache(string id);
}