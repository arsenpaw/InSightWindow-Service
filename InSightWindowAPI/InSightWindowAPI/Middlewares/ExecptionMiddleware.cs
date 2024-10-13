using System.Net;
using System.Text.Json;
using System.Net;
using System.Text.Json;
namespace InSightWindowAPI.Middlewares;

public static class AbstractExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AbstractExceptionHandlerMiddleware>();
    }
}

public class AbstractExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public AbstractExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            // Get the status code and message
            var (status, message) = GetResponse(exception);
            response.StatusCode = (int)status;

            // Return JSON error message
            var errorResponse = new
            {
                StatusCode = response.StatusCode,
                Message = message
            };
            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

    private (HttpStatusCode, string) GetResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, "Required argument was null."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access is denied."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }
}