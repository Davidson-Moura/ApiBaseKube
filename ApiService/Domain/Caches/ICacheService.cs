namespace ApiService.Domain.Caches
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        string GetKey(CachePrefix prefix, string enterpriserId, string entityId);
    }
    public enum CachePrefix
    {
        auth_group = 0,
        enterpriser = 1,
    }
}
