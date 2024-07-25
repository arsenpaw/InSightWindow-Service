using Microsoft.AspNetCore.SignalR;

namespace InSightWindowAPI.Serivces
{
    public class PushNotificationService : IPushNotificationService
    {

        private readonly ILogger<PushNotificationService> _logger;


        public PushNotificationService(ILogger<PushNotificationService> logger )
        {
            _logger = logger;

        }

        public Task SendNotificationToUser(Guid userId)
        {
            return Task.CompletedTask;
        }
    }
}
