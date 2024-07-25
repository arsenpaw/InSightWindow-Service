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
        private readonly UsersContext _context;

        public FireBaseTokensController(UsersContext context)
        {
            _context = context;
        }


        // GET: api/FireBaseTokens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ICollection<string>>> GetFireBaseToken(Guid id)
        {
          if (_context.FireBaseTokens == null)
          {
              return NotFound();
          }
            var fireBaseTokenList = await _context.FireBaseTokens.Where(x => x.UserId == id).Select(f => f.Token).ToListAsync();

            if (fireBaseTokenList == null)
            {
                return NotFound();
            }

            return fireBaseTokenList;
        }

        // PUT: api/FireBaseTokens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFireBaseToken(Guid id, FireBaseToken fireBaseToken)
        {
            if (id != fireBaseToken.Id)
            {
                return BadRequest();
            }

            _context.Entry(fireBaseToken).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FireBaseTokenExists(id))
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
        public async Task<ActionResult<FireBaseToken>> PostFireBaseToken(string token)
        {
            //mb validate firebase token
            var userId = HttpContext.GetUserIdFromClaims();
            var oldUserTokens = await _context.FireBaseTokens.Where(x => x.UserId == userId).Select(f => f.Token).ToListAsync();
            if (oldUserTokens.Contains(token))
            {
                return StatusCode(204, "Already exists");
            }

            FireBaseToken fireBaseToken = new FireBaseToken
            {
                UserId = userId,
                Token = token
            };
            _context.FireBaseTokens.Add(fireBaseToken);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFireBaseToken", new { id = fireBaseToken.Id }, fireBaseToken);
        }

        // DELETE: api/FireBaseTokens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFireBaseToken(Guid id)
        {
            if (_context.FireBaseTokens == null)
            {
                return NotFound();
            }
            var fireBaseToken = await _context.FireBaseTokens.FindAsync(id);
            if (fireBaseToken == null)
            {
                return NotFound();
            }

            _context.FireBaseTokens.Remove(fireBaseToken);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FireBaseTokenExists(Guid id)
        {
            return (_context.FireBaseTokens?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
