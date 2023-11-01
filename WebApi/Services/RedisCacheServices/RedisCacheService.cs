using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace WebApi
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RedisCacheService> _loggerService;
        public RedisCacheService(
            IDistributedCache distributedCache,
            ILogger<RedisCacheService> loggerService)
        {
            _distributedCache = distributedCache;
            _loggerService = loggerService;
        }

        public async Task AddAsync<T>(string key, T entity, DateTimeOffset exprirationTime, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var jsonSerializerOption = new JsonSerializerOptions() { WriteIndented = true };
                var jsonObject = JsonSerializer.Serialize(entity, jsonSerializerOption);

                var cacheOption = new DistributedCacheEntryOptions { AbsoluteExpiration = exprirationTime };
                await _distributedCache.SetStringAsync(key, jsonObject, cacheOption, cancellationToken);
            }
            catch (Exception exception)
            {
                _loggerService.LogError("Redis error: {0}",exception.Message);
            }
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var dataInCache = await _distributedCache.GetStringAsync(key, cancellationToken);

                return !string.IsNullOrEmpty(dataInCache) ? JsonSerializer.Deserialize<T>(dataInCache) : default;
            }
            catch (Exception exception)
            {
                _loggerService.LogError("Redis error: {0}", exception.Message);
                return default;
            }
        }

        public async Task RemoveByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception exception)
            {
                _loggerService.LogError("Redis error: {0}", exception.Message);
            }
        }
    }
}

