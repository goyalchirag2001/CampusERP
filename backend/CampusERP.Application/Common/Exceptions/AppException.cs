using System.Net;

namespace CampusERP.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string code, string message, HttpStatusCode statusCode): base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public string Code { get; }

    public HttpStatusCode StatusCode { get; }
}