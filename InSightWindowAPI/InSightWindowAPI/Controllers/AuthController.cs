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
    public class AuthController : ControllerBase
    {
        private readonly UsersContext _context;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public AuthController(UsersContext context, IMapper mapper, JwtSettings jwtSettings)
        {
            _context = context;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
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

            return Ok(user);
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

            if (refreshToken.ExpitedDate < DateTime.UtcNow)
            {
                var newRefreshToken = await GenerateRefreshToken();
                _mapper.Map(newRefreshToken, refreshToken);
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
            var oldRefreshTokenObj = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken.ToString());

            if (oldRefreshTokenObj == null)
                return Unauthorized("Invalid refresh token");
            if (oldRefreshTokenObj.ExpitedDate < DateTime.UtcNow)
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

        private async Task<ObjectResult> CreatResponceWithTokens(string token, RefreshToken refreshToken)
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



        private async Task<string> GenerateToken(User user)
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
    }


}
