using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InSightWindowAPI.JwtSetting;
using InSightWindowAPI.Models.Dto;
using InSightWindowAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using AutoMapper;
namespace InSightWindowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UsersContext _context;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthController> _logger;   
        public AuthController(UsersContext context, IMapper mapper, JwtSettings jwtSettings, ILogger<AuthController> logger)
        {
            _context = context;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
            _logger = logger;
        }
        [HttpGet("test")]
        public async Task<IActionResult> TestQuery()
        {
            var t =  _context.Users.GroupJoin(_context.FireBaseTokens,
                 user => user.Id,
                 device => device.UserId,
                 (user, devices) => new
                 {
                     User = user,
                     Devices = devices
                 })
                 .Where(model => model.Devices.Count() != 0).ToList();


            return Ok(t);
        }
        [HttpPost("create")]
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
       
            _logger.LogInformation("User with {Email} has registered in ", user.Email);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(UserLoginDto userLogin)
        {
            _logger.LogWarning(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOGS.txt"));
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLogin.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                return Unauthorized("Invalid email or password.");
            }
            var token = await GenerateToken(user);
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (refreshToken.ExpitedDate < DateTime.UtcNow)
            {
                var newRefreshToken = await GenerateRefreshToken();
                _mapper.Map(newRefreshToken, refreshToken);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("User with enail: {Email} has logged in ", user.Email);
            return await CreatResponceWithTokens(token, refreshToken);
        }
        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<string>> RefreshTokens()
        {
            if (!Request.Headers.TryGetValue("refresh-token", out var refreshToken))
            {
                return BadRequest("Refresh token is missing");
            }
            var oldRefreshTokenObj = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken.ToString());
            
            Guid requestingUser = HttpContext.GetUserIdFromClaims();
 
            _logger.Log(LogLevel.Information, "{requestingUser} user id was retrived from claim ", requestingUser);
            if (oldRefreshTokenObj == null)
            {
                _logger.Log(LogLevel.Warning, "{requestingUser} has provided bad refresh token", requestingUser);
                return Unauthorized("Invalid refresh token");
            }
            else if (requestingUser != oldRefreshTokenObj.UserId)
            {
                _logger.Log(LogLevel.Warning, "{requestingUser} has provided other refresh refresh token", requestingUser);
                return Unauthorized("Invalid operations");
            }
            if (oldRefreshTokenObj.ExpitedDate < DateTime.UtcNow)
            {
                _logger.Log(LogLevel.Warning, "{requestingUser} token has expired", requestingUser);
                return Unauthorized("Token expired");
            }

            //update refresh token
            RefreshToken newRefreshToken;
            if (oldRefreshTokenObj.ExpitedDate < DateTime.UtcNow.AddMonths(2))
            {
                newRefreshToken = await GenerateRefreshToken();
                _mapper.Map(newRefreshToken, oldRefreshTokenObj);
                await _context.SaveChangesAsync();
                _logger.Log(LogLevel.Information, "{requestingUser} has succesfully update their tokens", requestingUser);
            }
            else { newRefreshToken = oldRefreshTokenObj; }
            //update default token
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == newRefreshToken.UserId);
            string token = await GenerateToken(user);

            _logger.Log(LogLevel.Information, "{requestingUser} has succesfully receive tokens", requestingUser);
            return await CreatResponceWithTokens(token, newRefreshToken);

        }

        private async Task<ObjectResult> CreatResponceWithTokens(string token, RefreshToken refreshToken)
        {
            var result = new ObjectResult(Ok());
            Response.Headers.Add("Access-Control-Expose-Headers", "token,refresh-token");
            Response.Headers.Add("token", token);
            Response.Headers.Add("refresh-token", refreshToken.Token);
            Response.Cookies.Append("refresh-token", refreshToken.Token, new CookieOptions
            {
                Domain = "localhost",
                Expires = refreshToken.ExpitedDate,
                IsEssential = true,
                  Path = "/",
                  SameSite = SameSiteMode.Strict
                  

            });
            Response.Cookies.Append("token", token.ToString(), new CookieOptions
            {
                Domain = "localhost",
                IsEssential = true,
                Expires = refreshToken.ExpitedDate,
                Path = "/",
                SameSite = SameSiteMode.Strict


            });
            
            return result;
        }

        private async Task<RefreshToken> GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpitedDate = DateTime.UtcNow.AddMonths(6),
               
            };
            return refreshToken;
        }

        private async Task<string> GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
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
    }


}
