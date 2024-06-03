using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using AutoMapper;
using Newtonsoft.Json;

namespace InSightWindowAPI.Controllers
{
    public class NoChanges : IActionResult
    {
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 208;
            await context.HttpContext.Response.WriteAsync("No changes were made.");
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UsersDbController : ControllerBase
    {
        private readonly UsersContext _context;
        private readonly IMapper _mapper;

        public UsersDbController(UsersContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/UsersDb
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/UsersDb/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/UsersDb/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {   
          
            var foundUser = await _context.Users.FindAsync(id);
            var mappedOlduser = JsonConvert.SerializeObject( _mapper.Map<User>(foundUser));
            var mappedNewuser = JsonConvert.SerializeObject(_mapper.Map<User>(user));
            if (mappedOlduser == mappedNewuser) { return new NoChanges(); }
            if (foundUser == null) { return NotFound(); }
            //  _context.Entry(mappedOlduser).State = EntityState.Modified;
            try
            {
                foundUser.Name = user.Name;
                foundUser.Password = user.Password;
                Debug.WriteLine(_context.Entry(foundUser).State);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Data);
            }

            return Ok();
        }
        [HttpPut("BindTo/{userId}/{deviceId}")]
        public async Task<IActionResult> PutDevice(Guid userId, Guid deviceId)
        {
            var userToBind = await _context.Users.FindAsync(userId);
            var deviceToBind = await _context.Devices.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == deviceId);
            if (userToBind == null || deviceToBind == null)
            {
                return NotFound();
            }
            userToBind.Devices.Add(deviceToBind);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(deviceToBind);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while saving the changes.");
            }

            return NoContent();
        }
        // POST: api/UsersDb
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> CreatUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'UsersContext.Users'  is null.");
          }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/UsersDb/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
