using System.Text.Json;
using CampusERP.Application.Common.Exceptions;

namespace CampusERP.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;

        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "[{Code}] {Message}", ex.Code, ex.Message);

            await WriteResponseAsync(context, (int)ex.StatusCode, ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[INTERNAL_SERVER_ERROR] {Message}", ex.Message);

            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "INTERNAL_SERVER_ERROR", "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string code, string message)
    {
        context.Response.StatusCode = statusCode;

        context.Response.ContentType = "application/json";

        var response = new
        {
            Success = false,

            StatusCode = statusCode,

            Code = code,

            Message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}