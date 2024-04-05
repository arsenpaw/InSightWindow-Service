using Microsoft.AspNetCore.SignalR;
using InSightWindowAPI.Controllers;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using InSightWindowAPI.Storage;

namespace InSightWindowAPI.Controllers
{
    public class ClientStatusHub : Hub
    {
        IMemoryCache _cache;
        public ClientStatusHub(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public async Task SendWindowStatusObject(WindowStatus windowStatus)
        {
            try
            {
                
                await Clients.All.SendAsync("ReceiveWindowStatus", windowStatus);

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Data);

            }

        }

        public async Task SendWindowStatus(string message)
        {
            var windowStatus = JsonConvert.DeserializeObject<WindowStatus>(message);
            await Clients.All.SendAsync("ReceiveWindowStatus", windowStatus);

        }

        public async Task SaveUserInput(UserInputStatus userInputStatus)
        {
            CacheManager cacheManager = new CacheManager();
            await cacheManager.WriteDataToCahe(_cache, 360, userInputStatus);
        }

    }
}
