using System.Text.Json;

namespace OrderManagement.API.Middleware;

public class SimpleExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public SimpleExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = exception switch
        {
            KeyNotFoundException => 404,
            ArgumentException => 400,
            _ => 500
        };

        context.Response.ContentType = "application/json";
        var response = new { error = exception.Message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public static class SimpleExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseSimpleExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SimpleExceptionMiddleware>();
    }
} 
