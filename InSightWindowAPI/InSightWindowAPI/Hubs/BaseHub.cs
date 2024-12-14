using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace InSightWindowAPI.Hubs
{
    public class BaseHub : Hub
    {
        //Later device will have JWT so only one identifier will be needed
        public Guid UserId => Guid.TryParse(Context?.User?.Identity?.Name, out var userId) ? userId : Guid.Empty;

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
