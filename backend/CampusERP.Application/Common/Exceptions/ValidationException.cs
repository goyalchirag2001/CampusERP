using System.Net;

namespace CampusERP.Application.Common.Exceptions;

public sealed class ValidationException : AppException
{
    public ValidationException(string code, string message) : base(code, message, HttpStatusCode.BadRequest)
    {
    }
}