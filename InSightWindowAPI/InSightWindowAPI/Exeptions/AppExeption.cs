using InSightWindowAPI.Exceptions;
using System.Net;

namespace InSightWindowAPI.Exeptions
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorCode { get; }

        public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }
        public AppException(ExceptionMessage msg) : base(msg.Message)
        {
            StatusCode = msg.StatusCode;

        }
    }

}
