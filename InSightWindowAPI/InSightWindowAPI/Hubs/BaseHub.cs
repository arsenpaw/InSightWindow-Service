using InSightWindowAPI.Serivces;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace InSightWindowAPI.Hubs
{
    public class BaseHub: Hub
    {
        public IAesService AesService { get; set; }

        public BaseHub(IAesService aesService)
        {
            AesService = aesService;
        }

        public Guid DeviceId
        {
            get
            {
                var userIdClaim = Context.GetHttpContext().Request.Headers["DeviceId"].ToString(); 
                return userIdClaim != null && Guid.TryParse(userIdClaim, out var userId)
                    ? userId
                    : Guid.Empty;
            }
        }

    }
}
