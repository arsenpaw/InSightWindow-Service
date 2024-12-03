using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using InSightWindowAPI.Exceptions;
using InSightWindowAPI.Exeptions;

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
    private readonly ILogger<AbstractExceptionHandlerMiddleware> _logger;

    public AbstractExceptionHandlerMiddleware(RequestDelegate next, ILogger<AbstractExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            // Handle custom application exception
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            // Log unhandled exception
            _logger.LogError(ex, "Unhandled exception occurred.");

            // Handle generic exception
            var (status, message) = MapExceptionToResponse(ex);
            await HandleExceptionAsync(context, status, message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new BaseErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private (HttpStatusCode, string) MapExceptionToResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, "A required argument is missing."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "You are not authorized to access this resource."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }
}

public class BaseErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
}
