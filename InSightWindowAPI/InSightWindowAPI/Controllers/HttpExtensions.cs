using System.Security.Claims;

namespace InSightWindowAPI.Controllers
{
    public static class HttpExtensions
    {
        public static Guid GetUserIdFromClaims(this HttpContext httpContext)
        {

            var identity = httpContext.User.Identity as ClaimsIdentity;

            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;

            // Gets name from claims. Generally it's an email address.
            var usernameClaim = claim.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            return new Guid(usernameClaim.Value);
        }
    }
}
