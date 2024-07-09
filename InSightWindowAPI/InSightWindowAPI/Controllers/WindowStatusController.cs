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

namespace InSightWindowAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WindowStatusController : ControllerBase
    {
        public IMemoryCache _cache { get;  set; }

        private readonly UsersContext _context;

        private readonly IHubContext<ClientStatusHub> _hubContext;

        private async Task<string> GetTargetUserIdOrDefault(Device device)
        {
            var foundDevice = await _context.Devices.FirstOrDefaultAsync(x => x.Id == device.Id);
            return foundDevice != null ? foundDevice.UserId.ToString() : null;
        }

        public WindowStatusController(IMemoryCache memoryCache, IHubContext<ClientStatusHub> hubContext, UsersContext context)
        {
            _cache = memoryCache;
            _hubContext = hubContext;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SendWidnowStatusToClient([FromBody] AllWindowDataDto windowStatus)
        {

            try
            {
                var userId = await GetTargetUserIdOrDefault(windowStatus);
                if (userId != null)
                {
                    await _hubContext.Clients.User(userId).SendAsync("ReceiveWindowStatus", windowStatus);
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
                    return StatusCode(500,"No data");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

       


    }
}