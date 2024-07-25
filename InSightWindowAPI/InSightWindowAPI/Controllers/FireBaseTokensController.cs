using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace InSightWindowAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FireBaseTokensController : ControllerBase
    {
        private readonly ILogger<FireBaseTokensController> _loger;
        private readonly UsersContext _context;

        public FireBaseTokensController(UsersContext context, ILogger<FireBaseTokensController> logger)
        {
            _context = context;
            _loger = logger;
        }



        // PUT: api/FireBaseTokens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFireBaseToken(Guid id, FireBaseToken fireBaseToken)
        { 
            _context.Entry(fireBaseToken).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (true)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FireBaseTokens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{token}")]
        public async Task<ActionResult<FireBaseToken>> SetUserToken(string token)
        {
            _loger.LogInformation("Manage user token");
            try
            {
                var userId = HttpContext.GetUserIdFromClaims();
                var oldUserTokens = await _context.FireBaseTokens.FirstOrDefaultAsync(x => x.UserId == userId);
                if (oldUserTokens != null && oldUserTokens.Token == token)
                {
                    return Ok();
                }
                else if (oldUserTokens != null && oldUserTokens.Token != null)
                {
                    oldUserTokens.Token = token;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    FireBaseToken fireBaseToken = new FireBaseToken
                    {
                        UserId = userId,
                        Token = token
                    };
                    _context.FireBaseTokens.Add(fireBaseToken);
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
