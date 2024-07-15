using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using InSightWindowAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using InSightWindowAPI.Models;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI.Models.DeviceModel;

namespace InSightWindowAPI.Hubs
{
    [Authorize]
    public class ClientStatusHub : Hub
    {
        private readonly UsersContext _context;
        private IMemoryCache _cache;
        private ILogger<ClientStatusHub> _logger;

        public ClientStatusHub(UsersContext context, IMemoryCache memoryCache, ILogger<ClientStatusHub> logger)
        {
            _context = context;
            _cache = memoryCache;
            _logger = logger;
        }

        public async Task<string> GetTargetUserIdOrDefault(Guid deviceId)
        {
            var foundDevice = await _context.Devices.FirstOrDefaultAsync(x => x.Id == deviceId);
            if (foundDevice == null) { return null ; }
            return foundDevice.UserId.ToString();
        }

        public async Task SaveUserInput(UserInputStatus userInputStatus)
        {
            _cache.Set(userInputStatus.DeviceId.ToString(), userInputStatus);
        }

        public override  Task OnConnectedAsync()
        {
            Console.WriteLine("new user connected");
            _logger.Log(LogLevel.Information, "User connected");
            return base.OnConnectedAsync();
        }

        public void Test(string deviceId) 
        {
            _logger.Log(LogLevel.Information, "Test method invoked");
            Debug.WriteLine(deviceId);  
        }
        public async Task<string> SendUserInputToTargetDevice(UserInputStatus userInputStatus)
        {
            if (userInputStatus == null) { _logger.Log(LogLevel.Information, "Null data received"); return "415 Unsuported Media Type"; }
            _logger.Log(LogLevel.Information, "Try to send data to device from hub");

            try
            {
                Guid userJWTId = new Guid(Context.UserIdentifier);
                var subscribedUser = await _context.Devices.Where(device => device.UserId == userJWTId).Select(colum => colum.Id).FirstOrDefaultAsync();
                if (subscribedUser == userJWTId)
                {
                    // send data to microcontroller}
                    _logger.Log(LogLevel.Information, "Data sened");
                    return "200 OK";
                }
                else
                {
                    _logger.Log(LogLevel.Warning, $"User {userJWTId} has tried to hack the sending system", userJWTId);
                    return "401 Unauthorized ";
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message);
                return "500 Internal Server Error ";
            }
        }

        public async Task<string> SendWidnowStatusToClient(string json)
        {
            _logger.Log(LogLevel.Information, "Data sending to user from hub");
            AllWindowDataDto windowStatus = JsonConvert.DeserializeObject<AllWindowDataDto>(json);
            if (windowStatus == null) { _logger.Log(LogLevel.Information, "Bad data achived while parse to WindowStatus in HUb"); return "415 Unsuported Media Type"; }
            _logger.Log(LogLevel.Information, $"Data after convert");
            try
            {
                var userId = await GetTargetUserIdOrDefault(windowStatus.Id);
                if (userId != null)
                {
                    await Clients.User(userId).SendAsync("ReceiveWindowStatus", windowStatus);
                    return "200 OK";
                }
                else
                    return "202 No user subscribed";
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message);
                return "500 Internal Server Error ";
            }
        }

      

    }
}
