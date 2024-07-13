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

        public async Task SendWidnowStatusToClient(string json)
        {
            _logger.Log(LogLevel.Information, "Data sending to user from controller");
            AllWindowDataDto windowStatus = JsonConvert.DeserializeObject<AllWindowDataDto>(json);
            if (windowStatus == null) { _logger.Log(LogLevel.Information, "Bad data achived while parse to WindowStatus in HUb"); return; }
            _logger.Log(LogLevel.Information, $"Data after convert");
            try
            {
                var userId = await GetTargetUserIdOrDefault(windowStatus.Id);
                if (userId != null)
                {
                    await Clients.User(userId).SendAsync("ReceiveWindowStatus", windowStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message);
            }
        }

      

    }
}
