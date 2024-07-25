using FirebaseAdmin.Messaging;
using InSightWindowAPI.Controllers;
using InSightWindowAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.Reactive;
using System.Security.Cryptography;

namespace InSightWindowAPI.Serivces
{
    public class PushNotificationService : IPushNotificationService
    {

        private readonly ILogger<PushNotificationService> _logger;
        private readonly UsersContext _context;

      


        public PushNotificationService(ILogger<PushNotificationService> logger, UsersContext contextFirebase)
        {
            _logger = logger;
            _context = contextFirebase;
        }

        public async Task SendNotificationToUser(Guid userId,string title,string body)
        {
            _logger.LogInformation("Sending notification to user with id: " + userId);
          
            string token = await _context.FireBaseTokens.Where(x => x.UserId == userId).Select(f => f.Token).FirstOrDefaultAsync();
            if (token == null)
            {
                _logger.LogInformation("No tokens found for user with id: " + userId);
                return;
            }
            try
            {
                var message = new Message()
                {
                    Token = token,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = body
                    }
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation("Successfully sent message: " + response); 
             
            }
            catch(Exception exs)
            {
                _logger.LogError("Error while sending notification to user with id: " + userId + " " + exs.Message);    
            }
            
         
        }
    }
}
