using Microsoft.AspNetCore.SignalR;
using InSightWindowAPI.Controllers;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace InSightWindowAPI.Controllers
{
    public class ClientStatusHub: Hub
    {

        public async Task SendWindowStatus()
        {
           IMemoryCache _cache =  CacheStorage.cache;
            try
            {
                // Check if the data exists in cache
                if (_cache.TryGetValue(nameof(WindowStatus), out WindowStatus windowStatus))
                {
                    Console.WriteLine("Data retrieved from cache successfully.");
                    await Clients.All.SendAsync("ReceiveWindowStatus", windowStatus);
                }
                else
                {
                    Console.WriteLine("Data not found in cache.");
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
        public async Task TestMe(string someRandomText)
        {
            await Clients.All.SendAsync(
                $"{this.Context.User.Identity.Name} : {someRandomText}",
                CancellationToken.None);
        }
    }
}
