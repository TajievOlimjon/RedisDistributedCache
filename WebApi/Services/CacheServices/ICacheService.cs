namespace WebApi
{ 
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool AddData<T>(string key,T entity,DateTimeOffset exprirationTime);
        bool DeleteDataByKey(string key);
    }
}


