namespace InSightWindowAPI.Middlewares
{
    public class AllowCredentiaksToSiteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AllowCredentiaksToSiteMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var allowOriginsSection = _configuration.GetSection("AllowedOrigin");
            var allowOrigins = allowOriginsSection.Exists()
                ? allowOriginsSection.Get<string[]>()?.ToList()
                : new List<string>();
            var requestOrigin = context.Request.Headers["Origin"].ToString();

            if (allowOrigins.Contains(requestOrigin))
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = requestOrigin;
                context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
            }
            else
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.Headers.Remove("Access-Control-Allow-Credentials");
            }

            await _next(context);
        }
    }

    public static class AllowCredentialsToSiteMiddlewareExtensions
    {
        public static IApplicationBuilder UseAllowCredentialsToSite(
                       this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AllowCredentiaksToSiteMiddleware>();
        }
    }
}
