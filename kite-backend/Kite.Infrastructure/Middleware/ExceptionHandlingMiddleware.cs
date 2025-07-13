using System.Net;
using System.Text.Json;
using Kite.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kite.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var error = exception switch
        {
            UnauthorizedAccessException => new Error("Auth.Unauthorized", "Unauthorized access"),
            ArgumentException => new Error("Request.Invalid", exception.Message),
            KeyNotFoundException => new Error("Resource.NotFound", "Resource not found"),
            _ => new Error("Server.InternalError", "An internal server error occurred")
        };

        context.Response.StatusCode = exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var result = Result<object>.Failure(error);
        var jsonResponse = JsonSerializer.Serialize(result);

        await context.Response.WriteAsync(jsonResponse);
    }
}