using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Security.AccessControl;
using InSightWindowAPI.Controllers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Data.Common;
using Websocket.Client;
using System.Data;

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
        public async Task<IActionResult> WriteDataToCacheAsync([FromBody] WindowStatus windowStatus)
        {

            try
            {
                DateTime cacheEntry;

                if (!_cache.TryGetValue(CacheKeys.Entry, out cacheEntry))
                {
                    cacheEntry = DateTime.Now;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                         .SetSlidingExpiration(TimeSpan.FromSeconds(3600));
                    _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
                }
                _cache.Set(nameof(WindowStatus), windowStatus);
                CacheStorage.cache = _cache;

                HubConnection hubConnection = new HubConnectionBuilder()
                 .WithUrl(new Uri("http://192.168.4.2:81/client-hub")) // This URL should match your SignalR hub endpoint
               // .WithUrl(new Uri("https://localhost:44324/client-hub")) // This URL should match your SignalR hub endpoint
                  .WithAutomaticReconnect()
                .Build();       
                
                await hubConnection.StartAsync();
                bool IsConnected = false;
                await hubConnection.SendAsync("SendWindowStatusObject",windowStatus);
                if (hubConnection?.State == HubConnectionState.Connected)
                {
                    IsConnected= true;
                }
                
                return Ok($"Data received:  T: {windowStatus.Temparature}, H {windowStatus.Humidity},WATER {windowStatus.WaterLevel}," +
                $"IS_PROTECTED {windowStatus.IsProtected}, IS_OPEN {windowStatus.IsOpen}, IS CONNECTED: {IsConnected}");
                
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