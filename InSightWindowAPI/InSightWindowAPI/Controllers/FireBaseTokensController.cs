using InSightWindowAPI.Serivces;
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

        public FireBaseTokensController(IFireBaseTokenService fireBaseRepository, ILogger<FireBaseTokensController> logger)
        {
            fireBaseTokenService = fireBaseRepository;
            _loger = logger;
        }


        // POST: api/FireBaseTokens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{token}")]
        public async Task<IActionResult> SetUserToken(string token)
        {
            await fireBaseTokenService.AddNewTokenToUser(token, UserId);
            return Ok();
        }



    }
}
