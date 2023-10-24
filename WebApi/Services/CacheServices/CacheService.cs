using StackExchange.Redis;
using System.Text.Json;

namespace WebApi
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;
        private readonly ILogger<CacheService> _logger;
        public CacheService(ILogger<CacheService> logger)
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb = redis.GetDatabase();
            _logger = logger;
        }
        public T GetData<T>(string key)
        {
            try
            {
                var value = _cacheDb.StringGet(key);
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

        public bool DeleteDataByKey(string key)
        {
            try
            {
                var value = _cacheDb.KeyExists(key);

                if (value)
                {
                    return _cacheDb.KeyDelete(key);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {ex.Message}", ex.Message);
                return false;
            }
        }

        public bool AddData<T>(string key, T entity, DateTimeOffset exprirationTime)
        {
            try
            {
                var expertyTime = exprirationTime.Subtract(DateTime.Now);
                var isSet = _cacheDb.StringSet(key, JsonSerializer.Serialize(entity), expertyTime);
                return isSet;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {ex.Message}", ex.Message);
                return false;
            }
        }
    }
}
