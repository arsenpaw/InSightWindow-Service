using FirebaseAdmin.Messaging;
using InSightWindowAPI.Controllers;
using InSightWindowAPI.Models;
using InSightWindowAPI.Serivces.Interfaces;
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

        public async Task SendNotificationToUser(Guid userId, string title, string body)
        {
            _logger.LogInformation("Sending notification to user with id: " + userId);

            var token = await _context.UserFireBaseTokens.Where(x => x.UserId == userId).Select(x => x.FireBaseToken).ToListAsync();
            if (token == null)
            {
                _logger.LogWarning("No tokens found for user with id: " + userId);
                return;
            }
            foreach (var item in token)
            {
                try
                {
                    var message = new Message()
                    {
                        Token = item.Token,
                        Notification = new FirebaseAdmin.Messaging.Notification()
                        {
                            Title = title,
                            Body = body
                        }
                    };

                    string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    _logger.LogInformation("Successfully sent message: " + response);

                }//temp
                catch (FirebaseMessagingException ex)
                {
                    _context.Remove(item);
                    _logger.LogError("Error while sending notification to user with id: " + userId + " " + ex.Message);
                }
                catch (Exception exs)
                {
                    _logger.LogError("Error while sending notification to user with id: " + userId + " " + exs.Message);
                }
                finally
                {
                    await _context.SaveChangesAsync();
                }
            }



        }
    }
}
