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
using InSightWindowAPI.Models.Dto;

namespace InSightWindowAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WindowStatusController : ControllerBase
    {
        public IMemoryCache _cache { get;  set; }

        //REMOVE THIS
        HubConnection hubConnection = new HubConnectionBuilder()
                  .WithUrl(new Uri($"http://192.168.4.2:81/client-hub")) // This URL should match your SignalR hub endpoint
                 //.WithUrl(new Uri("https://localhost:44324/client-hub")) // This URL should match your SignalR hub endpoint
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
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    await hubConnection.SendAsync("SendWindowStatusObject", windowStatus);
                    _cache.Set(nameof(WindowStatus), windowStatus);   
                    await hubConnection.StopAsync();
                    return Ok($"Data received:  T: {windowStatus.Temparature}, H {windowStatus.Humidity},WATER {windowStatus.isRain}," +
                    $"IS_PROTECTED {windowStatus.IsProtected}, IS_OPEN {windowStatus.IsOpen}");
                }
                else
                {
                    return BadRequest($"Server disconected from SinglR hub");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("getUserInput")]
        public async Task<IActionResult> GetUserInput()
        {
            try
            {
                var data = _cache.Get<UserInputStatus>(nameof(UserInputStatus));
                if (data != null)
                {
                    Console.WriteLine("Data retrieved from cache successfully.");
                    return Ok(data);
                }
                else
                {
                    Console.WriteLine("Data not found in cache.");
                    return StatusCode(500,"No data");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        
        public async Task<IActionResult> GetDataFromCash()
        {
            try
            {
                var data = _cache.Get<WindowStatus>(nameof(WindowStatus));
                if (data != null)
                {
                    Console.WriteLine("Data retrieved from cache successfully.");
                    return Ok(data);
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