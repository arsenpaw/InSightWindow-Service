using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI.Models;
using System.Diagnostics;

namespace InSightWindowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesDbController : ControllerBase
    {
        private readonly UsersContext _context;

        public DevicesDbController(UsersContext context)
        {
            _context = context;
        }

        // GET: api/DevicesDb
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
          if (_context.Devices == null)
          {
              return NotFound();
          }
            return await _context.Devices.ToListAsync();
        }

        // GET: api/DevicesDb/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(Guid id)
        {
          if (_context.Devices == null)
          {
              return NotFound();
          }
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // PUT: api/DevicesDb/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("BindTo/{userId}/{deviceId}")]
        public async Task<IActionResult> PutDevice(Guid userId, Guid deviceId)
        {
            var userToBind = await _context.Users.FindAsync(userId);
            var deviceToBind = await _context.Devices.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == deviceId);
            if (userToBind == null || deviceToBind == null)
            {
                return NotFound();
            }
            deviceToBind.UserId = userId;
            deviceToBind.User = userToBind;  
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

        // POST: api/DevicesDb
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(Device device)
        {
          if (_context.Devices == null)
          {
              return Problem("Entity set 'UsersContext.Devices'  is null.");
          }
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.Id }, device);
        }

        // DELETE: api/DevicesDb/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceExists(Guid id)
        {
            return (_context.Devices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
