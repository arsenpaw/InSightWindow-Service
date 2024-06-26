﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using InSightWindowAPI.Models.Dto;

namespace InSightWindowAPI.Hubs

{
    public class UserInputHub:Hub
    {
        IMemoryCache _cache;
        public UserInputHub(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public async Task SaveUserInput(UserInputStatus userInputStatus)
        {
            _cache.Set(nameof(UserInputStatus), userInputStatus);
            await Task.Run(() => SendUserInputResponce());
        }
        public async Task SendUserInputResponce()
        {
            await Task.Delay(2500);
            var userInputStatus = _cache.Get<WindowStatus>(nameof(WindowStatus));
            await Clients.All.SendAsync("ReceiveUserInputResponce", userInputStatus);
        }
    }
}
