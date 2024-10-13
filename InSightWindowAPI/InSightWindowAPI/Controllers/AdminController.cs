//using InSightWindowAPI.Enums;
//using InSightWindowAPI.Extensions;
//using InSightWindowAPI.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace InSightWindowAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize(Roles = UserRole.ADMIN)]
//    public class AdminController : ControllerBase
//    {
//        public readonly ILogger<AdminController> _logger;   

//        public readonly UsersContext _context;

//        public AdminController(ILogger<AdminController> logger, UsersContext context)
//        {
//            _logger = logger;   
//            _context = context;
//        }

//        [HttpPost("makeAdmin")]
//        public async Task<IActionResult> MakeAdmin([FromQuery] Guid targetUserId)
//        {
//            Guid userId = HttpContext.GetUserIdFromClaims();
//             _context.Roles.Where(Role => Role.UserId == targetUserId).FirstOrDefault().RoleName = UserRole.ADMIN;
//            await _context.SaveChangesAsync();
//            _logger.LogInformation(" {userId} have update {targetUserId} status: Admin", userId, targetUserId);


//            return Ok();
//        }
//    }
//}
