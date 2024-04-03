using Microsoft.Extensions.Caching.Memory;

namespace InSightWindowAPI.Storage
{
    public class CacheManager<T>
    {
  
        public async Task WriteDataToCahe(IMemoryCache _cache, int timeSec, T data)
        {
            
            DateTime cacheEntry;
            if (!_cache.TryGetValue(CacheKeys.Entry, out cacheEntry))
            {
                cacheEntry = DateTime.Now;
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                     .SetSlidingExpiration(TimeSpan.FromSeconds(timeSec));
                _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
            }
            _cache.Set(nameof(T), data);

        }

        public async Task<T> GetDataFromCache(IMemoryCache _cache)
        {
            if (_cache.TryGetValue(nameof(T), out T info))
            {
                Console.WriteLine("Data retrieved from cache successfully.");
                return await Task.FromResult<T>(info);
            }
            else
            {
                Console.WriteLine("Data not found in cache.");

                return (default(T));
            }

        }
        public static class CacheKeys
        {
            public static string Entry { get { return "_Entry"; } }
            public static string CallbackEntry { get { return "_Callback"; } }
            public static string CallbackMessage { get { return "_CallbackMessage"; } }
            public static string Parent { get { return "_Parent"; } }
            public static string Child { get { return "_Child"; } }
            public static string DependentMessage { get { return "_DependentMessage"; } }
            public static string DependentCTS { get { return "_DependentCTS"; } }
            public static string Ticks { get { return "_Ticks"; } }
            public static string CancelMsg { get { return "_CancelMsg"; } }
            public static string CancelTokenSource { get { return "_CancelTokenSource"; } }
        }
    }
}
