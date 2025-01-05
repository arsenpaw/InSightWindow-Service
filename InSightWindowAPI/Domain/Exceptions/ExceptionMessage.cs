using System.Net;

namespace InSightWindowAPI.Exceptions
{
    public class ExceptionMessage
    {
        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public ExceptionMessage(string message, HttpStatusCode? statusCode = null)
        {
            Message = message;
            StatusCode = statusCode ?? HttpStatusCode.BadRequest;
        }
    }
    public static class ExceptionMessages
    {
        public static readonly ExceptionMessage DeviceOccupied =
            new ExceptionMessage("The device is already occupied.", HttpStatusCode.Conflict);

        public static readonly ExceptionMessage NotFound =
            new ExceptionMessage("The requested resource does not exist.", HttpStatusCode.NotFound);

        public static readonly ExceptionMessage UnauthorizedAccess =
            new ExceptionMessage("You do not have permission to access this resource.", HttpStatusCode.Unauthorized);

        public static readonly ExceptionMessage ArgumentNull =
            new ExceptionMessage("A required argument was null.", HttpStatusCode.BadRequest);

        public static readonly ExceptionMessage UnexpectedError =
            new ExceptionMessage("An unexpected error occurred.", HttpStatusCode.InternalServerError);

        public static readonly ExceptionMessage NoSuchDeviceExist =
          new ExceptionMessage("No such device exist", HttpStatusCode.NotFound);

        public static readonly ExceptionMessage NoSuchUserExist =
          new ExceptionMessage("No such user exist", HttpStatusCode.NotFound);
    }
}
