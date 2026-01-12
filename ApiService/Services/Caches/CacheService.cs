using Microsoft.Extensions.Caching.Distributed;
using ApiService.Domain.Caches;
using System.Text.Json;

namespace ApiService.Services.Caches
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            var json = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, json, options);
        }
        public async Task<T> GetAsync<T>(string key)
        {
            var json = await _cache.GetStringAsync(key);
            if (json == null) return default!;
            if (typeof(T) is string) return (T)(object)json;

            if (json == null) return default!;

            return JsonSerializer.Deserialize<T>(json)!;
        }
        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
        public string GetKey(CachePrefix prefix, string enterpriserId, string entityId)
        {
            return $"{prefix.ToString()}:{enterpriserId}:{entityId}";
        }
    }
}
