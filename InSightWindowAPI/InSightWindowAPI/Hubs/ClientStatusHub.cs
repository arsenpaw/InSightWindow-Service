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

        public ClientStatusHub(UsersContext context)
        {
            _context = context;
        }
        public async Task<string> GetTargetUserIdOrDefault(Device device)
        {
             Device foundDevice =   await _context.Devices.FirstOrDefaultAsync(x => x.Id == device.Id);
            return foundDevice != null ? foundDevice.UserId.ToString() : null;
        }

        public override  Task OnConnectedAsync()
        {
         
            var userId = Context.UserIdentifier;
          
            return base.OnConnectedAsync();
        }

        public async Task SendWindowStatusObject(AllWindowDataDto windowStatus)
        {
            try
            {

                var userId = await GetTargetUserIdOrDefault(windowStatus);
                if (userId != null) 
                    await Clients.User(userId).SendAsync("ReceiveWindowStatus", windowStatus);
               
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Data);
            }
        }

        public async Task SendWindowStatus(string message)
        {
            var windowStatus = JsonConvert.DeserializeObject<AllWindowDataDto>(message);
            await Clients.All.SendAsync("ReceiveWindowStatus", windowStatus);
        }
    }
}
