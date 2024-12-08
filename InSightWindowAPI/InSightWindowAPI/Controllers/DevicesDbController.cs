using AutoMapper;
using InSightWindow.Models;
using InSightWindowAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InSightWindowAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DevicesDbController : BaseController
    {
        private readonly UsersContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DevicesDbController> _logger;
        private readonly IDeviceService _deviceService;

        public DevicesDbController(ILogger<DevicesDbController> logger, UsersContext context, IMapper mapper, IDeviceService deviceService)
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
            var deviceList = await _deviceService.GetUserDevices(UserId);
            return deviceList.ToList();

        }

        [HttpPost("Bind/{deviceId}")]
        public async Task<IActionResult> BindDevice([FromRoute] Guid deviceId)
        {
            await _deviceService.AddDeviceToUser(UserId, deviceId);

            return Ok();
        }


        [HttpPost("Unbind/{deviceId}")]
        public async Task<IActionResult> UnbindDevice([FromRoute] Guid deviceId)
        {
            await _deviceService.RemoveDeviceFromUser(UserId, deviceId);

            return Ok();
        }
    }
}
