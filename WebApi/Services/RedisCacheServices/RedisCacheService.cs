using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace WebApi
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _distributedCache;
        public RedisCacheService(IDistributedCache distributedCache) => _distributedCache = distributedCache;

        public async Task AddAsync<T>(string key, T entity, DateTimeOffset exprirationTime,CancellationToken cancellationToken = default) where T : class
        {
            var cacheOption = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = exprirationTime,
            };

            var jsonSerializerOption = new JsonSerializerOptions() { WriteIndented = true };
            var jsonObject = JsonSerializer.Serialize(entity,jsonSerializerOption);

            await _distributedCache.SetStringAsync(key, jsonObject, cacheOption, cancellationToken);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var dataInCache = await _distributedCache.GetStringAsync(key,cancellationToken);

            if (!string.IsNullOrEmpty(dataInCache))
            {
                return JsonSerializer.Deserialize<T>(dataInCache);
            }

            return default;
        }

        public async Task RemoveByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}
