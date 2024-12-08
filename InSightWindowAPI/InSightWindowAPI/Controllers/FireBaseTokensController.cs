using FirebaseAdmin.Messaging;
using InSightWindowAPI.Enums;
using InSightWindowAPI.Serivces;
using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InSightWindowAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FireBaseTokensController : BaseController
    {
        private readonly ILogger<FireBaseTokensController> _loger;
        private readonly IFireBaseTokenService fireBaseTokenService;
        private readonly IPushNotificationService _pushNotificationService;

        public FireBaseTokensController(IFireBaseTokenService fireBaseRepository, IPushNotificationService pushNotificationService, ILogger<FireBaseTokensController> logger)
        {
            fireBaseTokenService = fireBaseRepository;
            _pushNotificationService = pushNotificationService;
            _loger = logger;
        }



        [HttpGet("test/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage([FromRoute] string token)
        {
            var message = new Message()
            {
                Token = token,

                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    ImageUrl = "https://ideogram.ai/assets/publicly-available/image-1.jpg",
                    Title = "TestTitle",
                    Body = "TestBody"
                }
            };
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return Ok();
        }

        // POST: api/FireBaseTokens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{token}")]
        public async Task<IActionResult> SetUserToken(string token)
        {
            await fireBaseTokenService.AddNewTokenToUser(token, UserId);
            return Ok();
        }

        [HttpPost("send/{userId}")]
        [Authorize(Roles = UserRoles.ADMIN)]
        public async Task<IActionResult> SendMessage([FromRoute] Guid userId, [FromBody] Message message)
        {
            await _pushNotificationService.SendNotificationToUser(userId, message);
            return Ok();
        }



    }
}
