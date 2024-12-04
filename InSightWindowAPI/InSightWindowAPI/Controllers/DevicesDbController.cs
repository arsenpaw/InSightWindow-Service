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
using InSightWindowAPI.Serivces;

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
        private readonly IDeviceService _deviceService;  

        public DevicesDbController(ILogger<DevicesDbController> logger,UsersContext context,IMapper mapper, IDeviceService deviceService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _deviceService = deviceService;
        }

      

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDto>> GetDevice(Guid id)
        {
            var device = await _deviceService.GetDeviceById(id);
            return device;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(Guid id, DeviceDto device)
        {
           //TODO
           return Ok();

        }
        [HttpPost]
        public async Task<ActionResult<DeviceDto>> PostDevice(DeviceDto device)
        {
            var deviceDto = await _deviceService.CreateDevice(device);
            return deviceDto;
        }
   
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            await _deviceService.DeleteDevice(id);
            return Ok();
        }

        [HttpGet("DeviceOfUser")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDeviceList()
        {
            Guid userId = HttpContext.GetUserIdFromClaims();
            var deviceList = await _deviceService.GetUserDevices(userId);
            return deviceList.ToList();

        }

        [HttpPost("Bind/{deviceId}")]
        public async Task<IActionResult> BindDevice([FromRoute] Guid deviceId)
        {
            Guid userId = HttpContext.GetUserIdFromClaims();

            await _deviceService.AddDeviceToUser(userId, deviceId);

            return Ok();
        }


        [HttpPost("Unbind/{deviceId}")]
        public async Task<IActionResult> UnbindDevice([FromRoute] Guid deviceId)
        {
            Guid userId = HttpContext.GetUserIdFromClaims();

            await _deviceService.RemoveDeviceFromUser(userId, deviceId);

            return Ok();
        }
    }
}
