using Microsoft.AspNetCore.SignalR;
using InSightWindowAPI.Controllers;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace InSightWindowAPI.Controllers
{
    public class ClientStatusHub: Hub
    {
        

        public async Task SendWindowStatus(string message)
        {

            var windowStatus = JsonConvert.DeserializeObject<WindowStatus>(message);

           await Clients.All.SendAsync("ReceiveWindowStatus", windowStatus);
 
            
        }
        
    }
}
