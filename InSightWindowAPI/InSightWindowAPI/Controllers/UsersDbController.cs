using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI.Models;
using AutoMapper;
using InSightWindowAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using InSightWindowAPI.JwtSetting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InSightWindowAPI.Enums;
using System.Net;
using System.Net.Http;
using System.Web;
using Azure;
using System.Security.Cryptography;
using NuGet.Common;
using InSightWindow.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Identity;
using InSightWindowAPI.Filters;
using InSightWindowAPI.Serivces;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Extensions;


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
    [Authorize]
    [ValidateUserIdAsync("UsersDbController")]
    public class UsersDbController : ControllerBase
    {
        private readonly UsersContext _context;
        private readonly IMapper _mapper;
       
        private readonly ILogger<UsersDbController> _logger;    

        public UsersDbController(ILogger<UsersDbController> logger,UsersContext context, IMapper mapper)
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
            Guid id = HttpContext.GetUserIdFromClaims();    
            var user = await _context.Users.FindAsync(id);

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
            Guid id = HttpContext.GetUserIdFromClaims();
            var foundUser = await _context.Users.FindAsync(id);
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
            Guid id = HttpContext.GetUserIdFromClaims();
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var isUserHasDevices = await _context.Devices.AnyAsync(device => device.UserId == id);
            if (isUserHasDevices)
            {
                return Conflict("User has devices, please delete them first");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {@user} delete account", user );
            return Ok();
        }

        [HttpPost("BindTo")]
        public async Task<IActionResult> BindDevice([FromQuery] Guid deviceId)
        {

            Guid userId = HttpContext.GetUserIdFromClaims();
            
            var device = await _context.Devices.GroupJoin(_context.Users,
                            device => device.UserId,
                            user => user.Id,
                            (device, users) => new
                            {
                                Device = device,
                                Users = users
                            }).Where(model => model.Device.Id == deviceId && model.Device.UserId == null).Select(model => model.Device).FirstOrDefaultAsync();

            if (device == null)
            {
                return NotFound();
            }

            device.UserId = userId;
            await _context.SaveChangesAsync();
            _logger.LogInformation(" {userId} have added device {DeviceId}", device.Id, userId);
            var deviceDto = _mapper.Map<DeviceDto>(device);

            return Ok(deviceDto);
        }

        [HttpPost("UnbindFrom")]
        public async Task<IActionResult> UnbindDevice([FromQuery] Guid deviceId)
        {

            Guid userId = HttpContext.GetUserIdFromClaims();

            var device = await _context.Devices.GroupJoin(_context.Users,
                            device => device.UserId,
                            user => user.Id,
                            (device, users) => new
                            {
                                Device = device,
                                Users = users
                            }).Where(model => model.Device.Id == deviceId && model.Device.UserId == userId ).Select(model => model.Device).FirstOrDefaultAsync();

            if (device == null)
            {
                return NotFound();
            }

            device.UserId = null;
            await _context.SaveChangesAsync();
            _logger.LogInformation("{userId} unbind {@device}", device,userId);
            var deviceDto = _mapper.Map<DeviceDto>(device);

            return Ok(deviceDto);
        }


        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
