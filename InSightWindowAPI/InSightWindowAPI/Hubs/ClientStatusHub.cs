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

namespace InSightWindowAPI.Hubs
{
    [AllowAnonymous]
    public class ClientStatusHub : Hub
    {
        private readonly UsersContext _context;
        private IMemoryCache _cache;
        private ILogger<ClientStatusHub> _logger;
        private readonly IPushNotificationService _pusher;


        public ClientStatusHub(UsersContext context, IMemoryCache memoryCache, ILogger<ClientStatusHub> logger, IPushNotificationService pusher)
        {
            _context = context;
            _cache = memoryCache;
            _logger = logger;
            _pusher = pusher;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId == null)
            {
                _logger.Log(LogLevel.Information, "User connected to hub without JWT token, some method could be unavalible");
                return base.OnConnectedAsync();
            }
            _logger.Log(LogLevel.Information, $"User {userId} connected to hub", userId);
            return base.OnConnectedAsync();
        }



        public void Test(string deviceId)
        {
            _logger.Log(LogLevel.Information, "Test method invoked");
            Debug.WriteLine(deviceId);
        }
        [Authorize]
        public async Task<string> SendUserInputToTargetDevice(UserInputStatus userInputStatus)
        {
            if (userInputStatus == null || userInputStatus.DeviceId == Guid.Empty) { _logger.Log(LogLevel.Information, "Null data received"); return "415 Unsuported Media Type"; }
            _logger.Log(LogLevel.Information, "Try to send data to device from hub");

            try
            {
                Guid userJWTId = new Guid(Context.UserIdentifier);
                //ffix this go to db later
                var subscribedUserId = await _context.Devices.Where(device => device.UserId == userJWTId && device.Id == userInputStatus.DeviceId).Select(colum => colum.Id).FirstOrDefaultAsync();
                if (subscribedUserId == userJWTId)
                {
                    // send data to microcontroller}

                    await Clients.User(userInputStatus.DeviceId.ToString()).SendAsync("ReceiveUserInput", userInputStatus);
                    _logger.Log(LogLevel.Information, "Data was sucesfully send from user{userJWTId} to gadget {userInputStatus.DeviceId}", userJWTId, userInputStatus.DeviceId);
                    return "200 OK";
                }
                else
                {
                    _logger.Log(LogLevel.Warning, "Suspicious activity detected from user  {userJWTId}", userJWTId);
                    return "401 Unauthorized ";
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message);
                return "500 Internal Server Error ";
            }
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
