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
        private IMemoryCache _cache;

        public ClientStatusHub(UsersContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public async Task<string> GetTargetUserIdOrDefault(Device device)
        {
            Device foundDevice = await _context.Devices.FirstOrDefaultAsync(x => x.Id == device.Id);
            return foundDevice != null ? foundDevice.UserId.ToString() : null;
        }

        public async Task SaveUserInput(UserInputStatus userInputStatus)
        {
            _cache.Set(userInputStatus.DeviceId.ToString(), userInputStatus);
        }

        public override  Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; 
            return base.OnConnectedAsync();
        }


    }
}
