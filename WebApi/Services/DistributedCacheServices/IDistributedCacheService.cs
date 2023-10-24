namespace WebApi
{
    public interface IDistributedCacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
            where T : class;
        Task AddAsync<T>(string key, T entity, DateTimeOffset exprirationTime, CancellationToken cancellationToken = default)
            where T : class;
        Task RemoveByKeyAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
    }
}




