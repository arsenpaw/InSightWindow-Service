﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using InSightWindowAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace InSightWindowAPI.Hubs
{
    [Authorize]
    public class ClientStatusHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
          
            return base.OnConnectedAsync();
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
    }
}
