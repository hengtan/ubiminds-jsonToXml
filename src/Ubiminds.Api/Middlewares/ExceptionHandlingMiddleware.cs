using System.Net;
using System.Text.Json;
using Serilog;

namespace Ubiminds.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context); // deixa passar
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "🔥 An unhandled exception occurred!");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                error = "Internal Server Error",
                details = ex.Message
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}