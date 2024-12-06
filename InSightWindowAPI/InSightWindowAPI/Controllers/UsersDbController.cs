using AutoMapper;
using InSightWindow.Models;
using InSightWindowAPI.Enums;
using InSightWindowAPI.Models;
using InSightWindowAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;


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
    public class UsersDbController : BaseController
    {
        private readonly UsersContext _context;
        private readonly IMapper _mapper;

        private readonly ILogger<UsersDbController> _logger;

        public UsersDbController(ILogger<UsersDbController> logger, UsersContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;

        }

        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            _logger.LogInformation("Test sucesfull !!!");
            return Ok("Test sucesfull !!!");
        }

        // GET: api/UsersDb
        [HttpGet]
        [Authorize(Roles = UserRoles.ADMIN)]
        public async Task<ActionResult<IEnumerable<UserRegisterDto>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.ToListAsync();
            var dtoUsers = _mapper.Map<List<UserRegisterDto>>(users);

            return Ok(dtoUsers);
        }

        // GET: api/UsersDb/5
        [HttpGet]
        [Route("concreteUser")]
        public async Task<ActionResult<UserRegisterDto>> GetUser()
        {
            var user = await _context.Users.FindAsync(UserId);

            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<UserRegisterDto>(user);

            return Ok(userDto);
        }

        // PUT: api/UsersDb/5
        [HttpPut]
        [Route("concreteUser")]
        public async Task<IActionResult> PutUser(UserRegisterDto user)
        {
            var foundUser = await _context.Users.FindAsync(UserId);
            if (foundUser == null)
            {
                return NotFound();
            }
            var oldUserToCompare = _mapper.Map<UserRegisterDto>(foundUser);
            var newUserToCompare = _mapper.Map<UserRegisterDto>(user);

            if (oldUserToCompare.Equals(newUserToCompare))
            {
                return new NoChanges();
            }

            _mapper.Map(user, foundUser);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User has updated info from {@oldUserToCompare} to {@newUserToCompare} ", oldUserToCompare, newUserToCompare);

            return NoContent();
        }

        [HttpDelete]
        [Route("concreteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }
            var isUserHasDevices = await _context.Devices.AnyAsync(device => device.UserId == UserId);
            if (isUserHasDevices)
            {
                return Conflict("User has devices, please delete them first");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {@user} delete account", user);
            return Ok();
        }
        //should be deleted in new versions
        [HttpPost("BindTo")]
        [SwaggerIgnore]
        public async Task<ActionResult<DeviceDto>> BindDevice([FromQuery] Guid deviceId)
        {

            var device = await _context.Devices.Where(x => x.Id == deviceId).FirstOrDefaultAsync();

            if (device == null)
                return NotFound("Devce Not Exist");

            device.UserId = UserId;
            await _context.SaveChangesAsync();
            _logger.LogInformation(" {userId} have added device {DeviceId}", device.Id, UserId);
            var deviceDto = _mapper.Map<DeviceDto>(device);

            return deviceDto;
        }
        //should be deleted in new versions
        [HttpPost("UnbindFrom")]
        [SwaggerIgnore]
        public async Task<IActionResult> UnbindDevice([FromQuery] Guid deviceId)
        {
            var device = await _context.Devices.Where(x => x.Id == deviceId).FirstOrDefaultAsync();

            if (device == null)
            {
                return NotFound();
            }

            device.UserId = null;
            await _context.SaveChangesAsync();
            _logger.LogInformation("{userId} unbind {@device}", device, UserId);
            var deviceDto = _mapper.Map<DeviceDto>(device);

            return Ok();
        }


        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
