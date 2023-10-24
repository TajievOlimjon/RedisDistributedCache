using System.Threading.Tasks;

namespace WebApi
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key,CancellationToken cancellationToken=default);
        Task AddAsync<T>(string key, T entity, DateTimeOffset exprirationTime, CancellationToken cancellationToken = default);
        Task RemoveByKeyAsync(string key, CancellationToken cancellationToken = default);
    }
}


