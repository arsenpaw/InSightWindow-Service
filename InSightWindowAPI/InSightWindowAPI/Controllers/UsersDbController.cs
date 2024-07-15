﻿using System;
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
     

        public UsersDbController(UsersContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private Guid GetUserIdFromClaims(HttpContext httpContext)
        {

            var identity = httpContext.User.Identity as ClaimsIdentity;

            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;

            // Gets name from claims. Generally it's an email address.
            var usernameClaim = claim.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            return new Guid(usernameClaim.Value);
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
        [HttpGet]
        [Route("concreteUser")]
        
        public async Task<ActionResult<UserDto>> GetUser()
        {
            Guid id = HttpContext.GetUserIdFromClaims();    
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
        [HttpPut]
        [Route("concreteUser")]
        public async Task<IActionResult> PutUser( UserDto user)
        {
            Guid id = HttpContext.GetUserIdFromClaims();
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
        [HttpDelete]
        [Route("concreteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            Guid id = HttpContext.GetUserIdFromClaims();
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
        //temporary
        [HttpPut("BindTo/{deviceId}")]
        public async Task<IActionResult> BindDevice( Guid deviceId)
        {
            Guid userId = HttpContext.GetUserIdFromClaims();
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


        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
