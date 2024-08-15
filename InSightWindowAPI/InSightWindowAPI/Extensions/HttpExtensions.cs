using System.Security.Claims;

namespace InSightWindowAPI.Extensions
{
    public static class HttpExtensions
    {
        public static Guid GetUserIdFromClaims(this HttpContext httpContext)
        {

            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
                return Guid.Empty;

            IEnumerable<Claim> claim = identity.Claims;
            var usernameClaim = claim.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            if (usernameClaim == null)
                return Guid.Empty;

            return new Guid(usernameClaim.Value);
        }
    }
}
