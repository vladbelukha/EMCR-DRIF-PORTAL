using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace EMCR.Utilities
{
    public static class IDistributedCacheEx
    {
        internal static string CachePrefix { get; set; } = string.Empty;

        public static async Task<T?> GetOrSet<T>(this IDistributedCache cache, string key, Func<Task<T>> factory, TimeSpan? expiry)
        {
            var obj = await cache.Get<T>(key);
            if (obj == null)
            {
                obj = await factory();
                await Set<T>(cache, key, obj, expiry);
            }

            return obj;
        }

        public static async Task<T?> Get<T>(this IDistributedCache cache, string key)
        {
            key = CacheKey(key);
            return Deserialize<T>(await cache.GetAsync(key));
        }

        public static async Task Set<T>(this IDistributedCache cache, string key, T obj, TimeSpan? expiry)
        {
            key = CacheKey(key);
            await cache.SetAsync(key, Serialize(obj), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry });
        }

        public static async Task Remove(this IDistributedCache cache, string key)
        {
            key = CacheKey(key);
            await cache.RemoveAsync(key);
        }

        private static string CacheKey(string key) => $"{CachePrefix}{key}";

        private static T? Deserialize<T>(byte[] data) => data == null || data.Length == 0 ? default(T?) : JsonSerializer.Deserialize<T?>(data);

        private static byte[] Serialize<T>(T obj) => obj == null ? Array.Empty<byte>() : JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    public static class CacheConfiguration
    {
        public static IServiceCollection AddCache(this IServiceCollection services, string cacheKeyPrefix)
        {
            IDistributedCacheEx.CachePrefix = string.IsNullOrEmpty(cacheKeyPrefix) ? string.Empty : $"{cacheKeyPrefix}-";
            services.AddDistributedMemoryCache();
            return services;
        }
    }
}
