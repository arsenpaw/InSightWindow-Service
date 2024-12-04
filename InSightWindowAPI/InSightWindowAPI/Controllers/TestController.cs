using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InSightWindowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : ControllerBase
    {
        IAesService AesService;

        public TestController(IAesService aesService)
        {
            AesService = aesService;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var t = "H/5gHQ037pvjn9qnEfUyVzMzy5BqAmZpC6Kkkz5svnsYGWR7VG1SccEmtLrcYFcmjXP389dw9SwH/EVzZ8lIIO+/42QUAx1t1WoOuh6U7saZw+MXlFH5FN9iQVum7zmL";
            var f = Convert.FromBase64String(t);
            string jsonData = AesService.DecryptStringFromBytes_Aes(f);
            return Ok();
        }
    }
}
