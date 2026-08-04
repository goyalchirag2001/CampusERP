using System.Net;

namespace CampusERP.Application.Common.Exceptions;

public sealed class BadRequestException : AppException
{
    public BadRequestException(string code, string message) : base(code, message, HttpStatusCode.BadRequest)
    {
    }
}
