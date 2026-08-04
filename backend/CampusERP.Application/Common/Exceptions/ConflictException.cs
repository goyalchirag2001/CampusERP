using System.Net;

namespace CampusERP.Application.Common.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string code, string message): base(code, message, HttpStatusCode.Conflict)
    {
    }
}