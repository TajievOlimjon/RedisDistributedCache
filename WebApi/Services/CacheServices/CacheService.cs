using StackExchange.Redis;
using System.Text.Json;

namespace WebApi
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;
        private readonly ILogger<CacheService> _logger;
        public CacheService(string? connection)
        {
            var redis = ConnectionMultiplexer.Connect("localhost:7003");
            _cacheDb = redis.GetDatabase();
        }
        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;
        }
        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await _cacheDb.StringGetAsync(key);
                if (!string.IsNullOrEmpty(value))
                {
                    var item = JsonSerializer.Deserialize<T>(value);
                    if (item != null) return item;
                    return default;
                }
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {ex.Message}", ex.Message);
                return default;
            }
        }

        public async Task RemoveByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await _cacheDb.KeyExistsAsync(key);

                if (value)
                {
                     await _cacheDb.KeyDeleteAsync(key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {ex.Message}", ex.Message);
            }
        }

        public async Task AddAsync<T>(string key, T entity, DateTimeOffset exprirationTime, CancellationToken cancellationToken = default)
        {
            try
            {
                var expertyTime = exprirationTime.Subtract(DateTime.Now);
                var isSet = await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(entity), expertyTime);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {ex.Message}", ex.Message);
            }
        }
    }
}
