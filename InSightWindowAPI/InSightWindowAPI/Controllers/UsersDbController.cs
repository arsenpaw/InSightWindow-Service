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
    public class UsersDbController : ControllerBase
    {
        private readonly UsersContext _context;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public UsersDbController(UsersContext context, IMapper mapper, JwtSettings jwtSettings)
        {
            _context = context;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
        }

        // GET: api/UsersDb
        [HttpGet]
        [Authorize(Roles = UserRole.ADMIN)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.ToListAsync();
            var dtoUsers = _mapper.Map<List<UserDto>>(users);

            return Ok(dtoUsers);
        }

        // GET: api/UsersDb/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
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
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        // PUT: api/UsersDb/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, UserDto user)
        {
            var foundUser = await _context.Users.FindAsync(id);

            if (foundUser == null)
            {
                return NotFound();
            }
            var oldUserToCompare = _mapper.Map<UserDto>(foundUser);
            var newUserToCompare = _mapper.Map<UserDto>(user);

            if (oldUserToCompare.Equals(newUserToCompare))
            {
                return new NoChanges();
            }

            _mapper.Map(user, foundUser);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        // CHANGE THIS LATER !!!!!!!!!!!!!
        [HttpDelete("{id}")]
        // [Authorize(Roles = UserRole.ADMIN)]
        [AllowAnonymous]
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

        [HttpPut("BindTo/{userId}/{deviceId}")]
        public async Task<IActionResult> BindDevice(Guid userId, Guid deviceId)
        {
            var user = await _context.Users.Include(u => u.Devices).FirstOrDefaultAsync(u => u.Id == userId);
            var device = await _context.Devices.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == deviceId);

            if (user == null || device == null)
            {
                return NotFound();
            }
            user.Devices.Add(device);
            await _context.SaveChangesAsync();
            var userDto = _mapper.Map<UserDto>(user);
            var deviceDto = _mapper.Map<DeviceDto>(device);

            return Ok(new { User = userDto.FirstName.Union(userDto.LastName), Device = deviceDto });
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateUser(UserDto user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'UsersContext.Users' is null.");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                return Conflict("This email has already been used.");
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var userEntity = _mapper.Map<User>(user);
            userEntity.RefreshToken = await GenerateRefreshToken();
            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = userEntity.Id }, user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> LoginUser(UserLoginDto userLogin)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLogin.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                return Unauthorized("Invalid email or password.");
            }
            var token = await GenerateToken(user);
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (refreshToken.ExpitedDate <DateTime.UtcNow)
            {
                var newRefreshToken = await GenerateRefreshToken();  
                _mapper.Map(newRefreshToken,refreshToken);
            }

            await _context.SaveChangesAsync();
            return await CreatResponceWithTokens(token, refreshToken);
        }
        [HttpPost("refresh-tokens")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> RefreshTokens()
        {
            if (!Request.Headers.TryGetValue("refresh-token", out var refreshToken))
            {
                return BadRequest("Refresh token is missing");
            }
            var oldRefreshTokenObj =  await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken.ToString());

           if (oldRefreshTokenObj == null) 
                 return Unauthorized("Invalid refresh token");
           if(oldRefreshTokenObj.ExpitedDate < DateTime.UtcNow)
                return Unauthorized("Token expired");
     
           //update refresh token
            var newRefreshToken = await GenerateRefreshToken();
            _mapper.Map(newRefreshToken, oldRefreshTokenObj); 
           

            //update default token
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == oldRefreshTokenObj.UserId);
            string token = await GenerateToken(user);

            await _context.SaveChangesAsync();



            return await CreatResponceWithTokens(token, newRefreshToken);

        }

        private async Task<ObjectResult> CreatResponceWithTokens(string token,RefreshToken refreshToken)
        {
            var result = new ObjectResult(Ok());
            Response.Headers.Add("token", token.ToString());
            Response.Headers.Add("refresh-token", refreshToken.Token.ToString());
            return result;
        }

    

        private async Task<RefreshToken> GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpitedDate = DateTime.UtcNow.AddMonths(3),
            };
            return refreshToken;
        }

        

        private async Task <string> GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
              
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
