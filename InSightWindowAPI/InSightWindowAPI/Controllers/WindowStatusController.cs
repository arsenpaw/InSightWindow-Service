using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Security.AccessControl;
using InSightWindowAPI.Controllers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Data.Common;
using InSightWindowAPI.Storage;
using Websocket.Client;
using System.Data;

namespace InSightWindowAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WindowStatusController : ControllerBase
    {
        private bool IsConnected;
        public IMemoryCache _cache { get; private set; }
        CacheManager cacheManager = new CacheManager();
        HubConnection hubConnection = new HubConnectionBuilder()
              .WithUrl(new Uri("http://192.168.4.2:81/client-hub")) // This URL should match your SignalR hub endpoint
                // .WithUrl(new Uri("https://localhost:44324/client-hub")) // This URL should match your SignalR hub endpoint
                 .WithAutomaticReconnect()
               .Build();
        public WindowStatusController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [HttpPost]
        public async Task<IActionResult> WriteDataToCacheAsync([FromBody] WindowStatus windowStatus)
        {

            try
            {
                await hubConnection.StartAsync();
                if (hubConnection?.State == HubConnectionState.Connected)
                {
                    await hubConnection.SendAsync("SendWindowStatusObject", windowStatus);
                    IsConnected = true;
                }
                else
                {
                    IsConnected = false;
                }
                await hubConnection.StopAsync();
                await cacheManager.WriteDataToCahe(_cache,100, windowStatus);
               // CacheStorage.storedCache = _cache;
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
                var data = cacheManager.GetDataFromCache(_cache);
                if  (data.Result != null)
                {
                    Console.WriteLine("Data retrieved from cache successfully.");
                    return Ok(data.Result);
                }
                else
                {
                    Console.WriteLine("Data not found in cache.");
                    return NotFound("Cash is empty");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}