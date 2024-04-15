using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

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
        }
    }
}
