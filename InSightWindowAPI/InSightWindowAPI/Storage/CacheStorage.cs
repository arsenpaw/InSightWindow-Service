using Microsoft.Extensions.Caching.Memory;

namespace InSightWindowAPI.Storage
{
    public static class CacheStorage
    {
        public static IMemoryCache storedCache {  get; set; }    

    }
}
