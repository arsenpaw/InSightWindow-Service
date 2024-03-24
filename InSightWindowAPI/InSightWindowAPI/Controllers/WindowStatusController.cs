using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Security.AccessControl;



namespace InSightWindowAPI.Controllers
{
    public static class CacheStorage
    {
        // Public property named 'cache' of type IMemoryCache.
        public static IMemoryCache cache { get; set; }
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
    [ApiController]
    [Route("[controller]")]
    public class WindowStatusController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public WindowStatusController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [HttpPost]
        public IActionResult WriteDataToCache([FromBody] WindowStatus windowStatus)
        {
            
            try
            {
                DateTime cacheEntry;

                // Look for cache key.
                if (!_cache.TryGetValue(CacheKeys.Entry, out cacheEntry))
                {
                    // Key not in cache, so get data.
                    cacheEntry = DateTime.Now;

                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                         // Keep in cache for this time, reset time if accessed.
                         .SetSlidingExpiration(TimeSpan.FromSeconds(3600));

                    // Save data in cache.
                    _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
                }
                _cache.Set(nameof(WindowStatus), windowStatus);
                CacheStorage.cache = _cache;
                return Ok($"Data received:  T: {windowStatus.Temparature}, H {windowStatus.Humidity},WATER {windowStatus.WaterLevel}," +
                    $"IS_PROTECTED {windowStatus.IsProtected}, IS_OPEN {windowStatus.IsOpen}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet]
        public IActionResult GetDataFromCash()
        {
            try
            {
                // Check if the data exists in cache
                if (_cache.TryGetValue(nameof(WindowStatus), out WindowStatus windowStatus))
                {
                    Console.WriteLine("Data retrieved from cache successfully.");
                    
                    return Ok(windowStatus);
                    //in dev
                }
                else
                {
                    Console.WriteLine("Data not found in cache.");
                    return Ok("Cash is empty");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

       
    }
}