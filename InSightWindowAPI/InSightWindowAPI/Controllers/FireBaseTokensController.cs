using InSightWindowAPI.Models;
using InSightWindowAPI.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InSightWindowAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FireBaseTokensController : BaseController
    {
        private readonly ILogger<FireBaseTokensController> _loger;
        private readonly UsersContext _context;

        public FireBaseTokensController(UsersContext context, ILogger<FireBaseTokensController> logger)
        {
            _context = context;
            _loger = logger;
        }


        // POST: api/FireBaseTokens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{token}")]
        public async Task<ActionResult<FireBaseToken>> SetUserToken(string token)
        {
            _loger.LogInformation("Manage user token");
            try
            {
                var oldUserTokens = await _context.UserFireBaseTokens.Where(x => x.UserId.Equals(UserId))
                    .Select(x => x.FireBaseToken).ToListAsync();
                if (oldUserTokens != null)
                {
                    return Ok();
                }
                else
                {
                    FireBaseToken fireBaseToken = new FireBaseToken
                    {
                        Token = token
                    };
                    oldUserTokens.Add(fireBaseToken);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception ex)
            {

                _loger.LogError(ex.Message);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
