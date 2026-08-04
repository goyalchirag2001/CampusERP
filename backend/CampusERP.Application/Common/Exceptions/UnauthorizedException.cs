using System.Net;

namespace CampusERP.Application.Common.Exceptions;

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string code, string message): base(code, message, HttpStatusCode.Unauthorized)
    {
    }
}