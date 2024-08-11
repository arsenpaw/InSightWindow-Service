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
using InSightWindowAPI.Hubs;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Models;
using InSightWindowAPI.Serivces;

namespace InSightWindowAPI.Controllers
{
    // this is debricated controller, it will probably will be deleted
    [ApiController]
    [Route("[controller]")]
    public class WindowStatusController : ControllerBase
    {
        public IMemoryCache _cache { get; set; }

        private readonly UsersContext _context;

        private readonly IHubContext<ClientStatusHub> _hubContext;

        public readonly IPushNotificationService _pusher;


        public WindowStatusController(IMemoryCache memoryCache, IHubContext<ClientStatusHub> hubContext, UsersContext context, IPushNotificationService push)
        {
            _cache = memoryCache;
            _hubContext = hubContext;
            _context = context;
            _pusher = push;
        }

        [HttpPost]
        public async Task<IActionResult> SendAlarm(Guid userId)
        {

            try
            {
                await _pusher.SendNotificationToUser(userId, "Alarm", "Alarm was triggered");
                return Ok();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Data);
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("getUserInput")]
        public async Task<IActionResult> GetUserInput(Guid deviceId)
        {
            try
            {
                var data = _cache.Get<UserInputStatus>(deviceId.ToString());
                if (data != null)
                {
                    Console.WriteLine("Data retrieved from cache successfully.");
                    return Ok(data);
                }
                else
                {
                    Console.WriteLine("Data not found in cache.");
                    return StatusCode(500, "No data");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("sendDeviceResponce")]
        public async Task<IActionResult> SetDeviceResponce(UserInputStatus deviceReply)
        {
            try
            {
                Guid? userId = await _context.Devices
               .Where(device => device.Id == deviceReply.DeviceId)
               .Select(device => device.UserId)
               .FirstOrDefaultAsync();
                if (userId != null)
                {
                    await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveUserInputResponce", deviceReply);
                    return Ok();
                }
                else
                    return NotFound("Cannot found user with this type of device");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Data);
                return StatusCode(500);
            }

        }
    }
}