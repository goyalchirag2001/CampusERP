using System.Net;

namespace CampusERP.Application.Common.Exceptions;

public sealed class BusinessRuleException : AppException
{
    public BusinessRuleException(string code, string message): base(code, message, HttpStatusCode.UnprocessableEntity)
    {
    }
}