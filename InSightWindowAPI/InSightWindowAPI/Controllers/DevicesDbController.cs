using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI.Models;
using System.Diagnostics;
using AutoMapper;
using InSightWindowAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindow.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using InSightWindowAPI.Extensions;

namespace InSightWindowAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DevicesDbController : ControllerBase
    {
        private readonly UsersContext _context;

        private readonly IMapper _mapper;  

        private readonly ILogger<DevicesDbController> _logger;

        public DevicesDbController(ILogger<DevicesDbController> logger,UsersContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDevices()
        {
          if (_context.Devices == null)
          {
              return NotFound();
          }
            List<Device> allDevices = await _context.Devices.ToListAsync<Device>();
             return _mapper.Map<List<DeviceDto>>(allDevices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDto>> GetDevice(Guid id)
        {
          if (_context.Devices == null)
          {
              return NotFound();
          }
            var device = _mapper.Map<DeviceDto>(await _context.Devices.FindAsync(id));

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(Guid id, DeviceDto device)
        {
            var deviceToChange = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id);

            if (deviceToChange == null ) { return NotFound();}
            
            try
            {
                _mapper.Map(device, deviceToChange);
                await _context.SaveChangesAsync();
                return Ok(device);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while saving the changes.");
            }

          
        }
        [HttpPost]
        public async Task<ActionResult<DeviceDto>> PostDevice(DeviceDto device)
        {

            _context.Devices.Add(_mapper.Map<Device>(device));
            await _context.SaveChangesAsync();

            return Ok(device);
        }

     
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
            try
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                Debug.WriteLine($"Failed to delete device {id}");
                return StatusCode(500, "Error, in deleting ");
            }
            
        }
        
        [HttpGet("DeviceOfUser")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDeviceList()
        {

            Guid userId = HttpContext.GetUserIdFromClaims();
            if (_context.Devices == null)
                return Problem("Entity set 'UsersContext.Devices' is null.");

            var userDevice = await _context.Users
            .Include(x => x.Devices)
            .Where(x => x.Id == userId)
            .Select(x => x.Devices)
            .FirstOrDefaultAsync();

            if (userDevice != null || !userDevice.Any())
            {
                return new List<DeviceDto>();
            }
            else
            {
                var deviceList = _mapper.Map<List<DeviceDto>>(userDevice);
                return deviceList;
            }

        }

        private bool DeviceExists(Guid id)
        {
            return (_context.Devices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
