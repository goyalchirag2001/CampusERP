using System.Net;

namespace CampusERP.Application.Common.Exceptions;

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string code, string message): base(code,message, HttpStatusCode.Forbidden)
    {
    }
}