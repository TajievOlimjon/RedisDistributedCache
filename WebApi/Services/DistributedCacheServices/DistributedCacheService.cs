using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;

namespace WebApi.Services.DistributedCacheServices
{
    public class DistributedCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private static ConcurrentDictionary<string, bool> CacheKeys = new();
        private readonly ApplicationDbContext _dbContext;
        public DistributedCacheService(
            IDistributedCache distributedCache,
            ApplicationDbContext dbContext)
        {
            _distributedCache = distributedCache;
            _dbContext = dbContext;
        }
        public async Task AddAsync<T>(string key, T entity, DateTimeOffset exprirationTime, CancellationToken cancellationToken = default) where T : class
        {
            string cachedValue = JsonSerializer.Serialize(entity);

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = exprirationTime,
            };
            await _distributedCache.SetStringAsync(key, cachedValue, options, cancellationToken);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            string? cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

            if (cachedValue is null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(cachedValue);
        }

        public async Task RemoveByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        public Task<bool> RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
