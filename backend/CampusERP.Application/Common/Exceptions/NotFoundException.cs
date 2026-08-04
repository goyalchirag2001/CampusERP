using System.Net;

namespace CampusERP.Application.Common.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string code, string message): base(code, message, HttpStatusCode.NotFound)
    {
    }
}