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
using InSightWindowAPI.Serivces;
using System.Runtime.Serialization.Formatters.Binary;

namespace InSightWindowAPI.Hubs
{
    [AllowAnonymous]
    public class ClientStatusHub : BaseHub
    {
        private readonly UsersContext _context;
        private IMemoryCache _cache;
        private ILogger<ClientStatusHub> _logger;
        private readonly IPushNotificationService _pusher;


        public ClientStatusHub(
       UsersContext context,
       IMemoryCache memoryCache,
       ILogger<ClientStatusHub> logger,
       IPushNotificationService pusher,
       IAesService aesService) : base(aesService)
        {
            _context = context;
            _cache = memoryCache;
            _logger = logger;
            _pusher = pusher;
         
        }

        public override Task OnConnectedAsync()
        {
            if (DeviceId == Guid.Empty)
            {
                _logger.Log(LogLevel.Information, "User connected to hub without Id");
                return base.OnConnectedAsync();
            }
            _logger.Log(LogLevel.Information, "Device {i} connected to hub", DeviceId);
            Clients.User(DeviceId.ToString());
            return base.OnConnectedAsync();
        }

        

        public async Task<int> ReceiveDataFromEsp32(string sensorData)
        {   
            byte[] sensorDataByte = Convert.FromBase64String(sensorData);
            Console.WriteLine(DeviceId);
            string jsonData = AesService.DecryptStringFromBytes_Aes(sensorDataByte);
            _logger.Log(LogLevel.Information, jsonData);
            var sensorDataDto = JsonConvert.DeserializeObject<SensorDataDto>(jsonData);
            if (sensorDataDto == null || DeviceId == Guid.Empty)
            {
                _logger.Log(LogLevel.Critical,
                    "No all credentials have detected while receive data from esp32, Data: {sdata}, DeviceId {uId}",sensorDataDto,DeviceId);
                return 405;
            }


            var subscribedUserId = await _context.Devices
                .Where(x => x.Id == DeviceId)
                .Select(x => x.UserId)
                .FirstOrDefaultAsync();

            if (subscribedUserId == Guid.Empty || subscribedUserId == null)
            {
                return 401;
            }
            // await Clients.User(userInputStatus.DeviceId.ToString()).SendAsync("ReceiveUserInput", userInputStatus);
            _logger.Log(LogLevel.Information, "Data was sucesfully send from user{userJWTId} to gadget {userInputStatus.DeviceId}",
                subscribedUserId, DeviceId);
             return 200;
        }
        private async Task<Guid> GetTargetUserIdOrDefault(Guid deviceId)
        {
            var foundDevice = await _context.Devices.FirstOrDefaultAsync(x => x.Id == deviceId);
            if (foundDevice == null)
                return Guid.Empty;
            return foundDevice.UserId.Value;


        }
        public async Task<string> SendWidnowStatusToClient(string json)
        {
            _logger.Log(LogLevel.Information, "Data sending to user from hub");
            var windowStatus = JsonConvert.DeserializeObject<AllWindowDataDto>(json);
            if (windowStatus == null)
            {
                _logger.Log(LogLevel.Information, "Bad data achived while parse to WindowStatus in HUb");
                return "415 Unsuported Media Type"; 
            }

            _logger.Log(LogLevel.Information, $"Data after convert");
            try
            {
                var userId = await GetTargetUserIdOrDefault(windowStatus.Id);
                if (!userId.Equals(Guid.Empty))
                {
                  if (windowStatus.isAlarm)
                    {
                        await _pusher.SendNotificationToUser(userId, "Alarm", "Alarm was triggered");
                    }
                    await Clients.User(userId.ToString()).SendAsync("ReceiveWindowStatus", windowStatus);
                    return "200 OK";
                }
                else
                    return "202  No user subscribed";
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message);
                return "500 Internal Server Error ";
            }
        }

      

    }
}
