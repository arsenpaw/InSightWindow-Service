using InSightWindowAPI.Exeptions;
using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace InSightWindowAPI.Hubs.Filters
{
    public class ErrorHandlingFilter : IHubFilter
    {
        private readonly ILogger<ErrorHandlingFilter> _logger;

        public ErrorHandlingFilter(ILogger<ErrorHandlingFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            try
            {
                return await next(invocationContext);
            }
            catch (AppException appException)
            {
                return appException.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=> An error occurred in hub method: {ex}", ex);
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
