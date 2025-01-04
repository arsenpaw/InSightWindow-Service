using FirebaseAdmin.Messaging;
using InSightWindowAPI.Models;
using InSightWindowAPI.Repository;
using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InSightWindowAPI.Serivces
{
    public class PushNotificationService : IPushNotificationService
    {

        private readonly ILogger<PushNotificationService> _logger;
        private readonly IFireBaseRepository _fireBaseRepository;



        public PushNotificationService(IFireBaseRepository fireBaseRepository, ILogger<PushNotificationService> logger)
        {
            _logger = logger;
            _fireBaseRepository = fireBaseRepository;
        }

        public async Task SendNotificationToUser(Guid userId, Message message)
        {
            _logger.LogInformation("Sending notification to user with id: " + userId);

            var token = await _fireBaseRepository.GetByUserId(userId).ToListAsync();
            foreach (var item in token)
            {
                try
                {
                    string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    _logger.LogInformation("Successfully sent message: " + response);

                }
                catch (FirebaseMessagingException ex)
                {
                    _fireBaseRepository.Remove(item.FireBaseToken);
                    _logger.LogError("Error while sending notification to user with id: " + userId + " " + ex.Message);
                    await _fireBaseRepository.SaveAsync();
                }
            }
        }
    }
}
