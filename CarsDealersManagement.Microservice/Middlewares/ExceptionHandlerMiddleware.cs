using CarsDealersManagement.Domain.Base;
using System.Net;
using System.Text.Json;

namespace CarsDealersManagement.Microservice.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex is FluentValidation.ValidationException
                ? (int)HttpStatusCode.BadRequest
                : (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new ErrorResponse
            {
                Title = (HttpStatusCode)context.Response.StatusCode == HttpStatusCode.BadRequest
                    ? "Bad Request"
                    : "Internal Server Error",
                Description = !string.IsNullOrEmpty(ex.Message)
                    ? ex.Message
                    : "An unexpected error occurred.",
                Status = (HttpStatusCode)context.Response.StatusCode
            });

            await context.Response.WriteAsync(result);
        }
    }
}