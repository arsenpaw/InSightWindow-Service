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
using Microsoft.AspNetCore.Cors;
namespace InSightWindowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    //  [RequireHttps]
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
        public async Task<IActionResult> TestQuery() {
            return Ok("Test query");
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
            return Ok();
        }



        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(UserLoginDto userLogin)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation(ipAddress);
          
            // _logger.LogWarning(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOGS.txt"));

            var user = await _context.Users.Include(user => user.Role)
                .Include(user => user.RefreshToken)
                .FirstOrDefaultAsync(u => u.Email == userLogin.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                return Unauthorized("Invalid email or password.");
            }
            var token = await GenerateToken(user);
            var refreshToken = user.RefreshToken;

            if (refreshToken.ExpitedDate < DateTime.UtcNow.AddMonths(2))
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

            if (oldRefreshTokenObj == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            Guid requestingUserId = oldRefreshTokenObj.UserId;
 
            _logger.Log(LogLevel.Information, "{requestingUserId} user id was retrived from refresh-token claim ", requestingUserId);
          
           
            if (oldRefreshTokenObj.ExpitedDate < DateTime.UtcNow)
            {
                _logger.Log(LogLevel.Warning, "{requestingUserId} token has expired", requestingUserId);
                return Unauthorized("Token expired");
            }

            //update refresh token
            RefreshToken newRefreshToken;
            if (oldRefreshTokenObj.ExpitedDate < DateTime.UtcNow.AddMonths(2))
            {
                newRefreshToken = await GenerateRefreshToken();
                _mapper.Map(newRefreshToken, oldRefreshTokenObj);
                await _context.SaveChangesAsync();
                _logger.Log(LogLevel.Information, "{requestingUserId} has succesfully update their tokens", requestingUserId);
            }
            else { newRefreshToken = oldRefreshTokenObj; }
            //update default token
            var user = await _context.Users.Include(user => user.Role).FirstOrDefaultAsync(x => x.Id == requestingUserId);
            string token = await GenerateToken(user);

            _logger.Log(LogLevel.Information, "{requestingUserId} has succesfully receive tokens", requestingUserId);
            return await CreatResponceWithTokens(token, newRefreshToken);

        }

        private async Task<ObjectResult> CreatResponceWithTokens(string token, RefreshToken refreshToken)
        {
            var result = new ObjectResult(Ok());
            Response.Headers.Add("Access-Control-Expose-Headers", "token,refresh-token");
            Response.Headers.Add("token", token);
            Response.Headers.Add("refresh-token", refreshToken.Token);
            //Response.Cookies.Append("refresh-token", refreshToken.Token, new CookieOptions
            //{
            //  //  Domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName,
            //  Domain = "localhost",
            //     Expires = refreshToken.ExpitedDate,
            //    IsEssential = true,
            //      Path = "/",
            //     SameSite  = SameSiteMode.None,
            //     Secure = true



            //});
            //Response.Cookies.Append("token", token.ToString(), new CookieOptions
            //{
            //   // Domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName,
            //    Domain = "localhost",
            //    IsEssential = true,
            //    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            //    Path = "/",
            //    SameSite = SameSiteMode.None,
            //    Secure = true


            //});
            
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
