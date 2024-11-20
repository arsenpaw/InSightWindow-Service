﻿using Microsoft.AspNetCore.SignalR;
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
using InSightWindowAPI.Models.Command;
using InSightWindowAPI.Hubs.ConnectionMapper;

namespace InSightWindowAPI.Hubs
{
    [AllowAnonymous]
    public class ClientStatusHub : BaseHub
    {
        private readonly UsersContext _context;
        private IMemoryCache _cache;
        private ILogger<ClientStatusHub> _logger;
        private readonly IPushNotificationService _pusher;
        private readonly static ConnectionMapping<Guid> _connectionMapping =
           new ConnectionMapping<Guid>();

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
                _logger.Log(LogLevel.Information, "No device was connected to hub");
                return base.OnConnectedAsync();
            }
            _logger.Log(LogLevel.Information, "Device {i} connected to hub", DeviceId);
            _connectionMapping.Add(DeviceId, Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var deviceId in _connectionMapping.GetConnections(DeviceId))
            {
                _connectionMapping.Remove(DeviceId, Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
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

            if (!subscribedUserId.HasValue)
                return 401; 
            
            _connectionMapping.Add(subscribedUserId.Value, Context.ConnectionId);
            // await Clients.User(userInputStatus.DeviceId.ToString()).SendAsync("ReceiveUserInput", userInputStatus);
            _logger.Log(LogLevel.Information, "Data was sucesfully send from user{userJWTId} to gadget {userInputStatus.DeviceId}",
                subscribedUserId, DeviceId);
             return 200;
        }
        
        //[Authorize]
        //public async Task<int> SendCommandToEsp32(Guid deviceId, CommandDto command)
         public async Task<int> SendCommandToEsp32()
        {
            Guid deviceId = Guid.Parse("6c1d08d1-4bac-44da-bdba-3165799c0497");
           var  command = new CommandDto { Command = CommandEnum.Open };

            _logger.Log(LogLevel.Information, "Data sending to user from hub");
            try
            {
                var connectionID = _connectionMapping.GetConnections(deviceId).FirstOrDefault();
                if (connectionID == null)
                {
                    _logger.Log(LogLevel.Warning, "No connection found for device {deviceId}", deviceId);
                    return 404;
                }
                await Clients.Client(connectionID).SendAsync("ReceiveCommand", JsonConvert.SerializeObject(command));
                return 200;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message);
                return 500;
            }
        }

      

    }
}
