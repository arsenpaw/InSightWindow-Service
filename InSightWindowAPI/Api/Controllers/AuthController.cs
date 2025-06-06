﻿using InSightWindowAPI.JwtSetting;
using InSightWindowAPI.Models.Dto;
using InSightWindowAPI.Models.Entity;
using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InSightWindowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;

        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">Registration details.</param>
        /// <returns>Action result.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return Conflict(new { message = "User already exists!" });

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign the "User" role to the newly registered user
            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { message = "User registered successfully!" });
        }

        /// <summary>
        /// Logs in a user and returns a JWT token along with a refresh token.
        /// </summary>
        /// <param name="model">Login details.</param>
        /// <returns>Access token and refresh token.</returns>
        [HttpPost("Login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] UserLoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid credentials." });

            var userRole = await _userManager.GetRolesAsync(user);
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, userRole);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            var response = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
            Response.Headers.Add("Access-Control-Expose-Headers", "token,refresh-token");
            Response.Headers.Add("token", accessToken);
            Response.Headers.Add("refresh-token", refreshToken.Token);
            return response;
        }

        /// <summary>
        /// Refreshes an access token using a valid refresh token.
        /// </summary>
        /// <param name="model">Refresh token request.</param>
        /// <returns>New access token and refresh token.</returns>
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var principal = GetPrincipalFromExpiredToken(model.AccessToken);
            if (principal == null)
                return BadRequest(new { message = "Invalid access token or refresh token." });

            var userId = Guid.Parse(principal.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return BadRequest(new { message = "Invalid access token or refresh token." });

            var refreshToken = await _tokenService.GetRefreshTokenAsync(model.RefreshToken);
            if (refreshToken == null || refreshToken.UserId != user.Id || !refreshToken.IsActive)
                return BadRequest(new { message = "Invalid refresh token." });


            await _tokenService.RevokeRefreshTokenAsync(refreshToken);

            var userRole = await _userManager.GetRolesAsync(user);
            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user, userRole);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            var response = new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };

            return Ok(response);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = new JwtSettings();
            _configuration.GetSection("JwtSettings").Bind(jwtSettings);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out var validatedToken);


                return validatedToken as JwtSecurityToken != null
                    ? new ClaimsPrincipal(new ClaimsIdentity(((JwtSecurityToken)validatedToken).Claims))
                    : null;
            }
            catch
            {
                return null;
            }
        }

    }
}
