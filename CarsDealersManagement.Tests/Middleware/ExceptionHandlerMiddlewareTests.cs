using CarsDealersManagement.Microservice.Middlewares;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace CarsDealersManagement.Tests.Middleware;

public class ExceptionHandlerMiddlewareTests
{
    private static async Task<(int StatusCode, string Body)> InvokeMiddleware(RequestDelegate next)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlerMiddleware(next);
        await middleware.Invoke(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        return (context.Response.StatusCode, body);
    }

    [Fact]
    public async Task Invoke_WhenNoException_PassesThroughSuccessfully()
    {
        var (statusCode, _) = await InvokeMiddleware(ctx =>
        {
            ctx.Response.StatusCode = 200;
            return Task.CompletedTask;
        });

        statusCode.Should().Be(200);
    }

    [Fact]
    public async Task Invoke_WhenUnauthorizedAccessException_Returns401()
    {
        var (statusCode, body) = await InvokeMiddleware(_ =>
            throw new UnauthorizedAccessException("Access denied."));

        statusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        body.Should().NotBeNullOrEmpty();

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("Title").GetString().Should().Be("Unauthorised");
        doc.RootElement.GetProperty("Description").GetString().Should().Be("Access denied.");
        doc.RootElement.GetProperty("Status").GetInt32().Should().Be(401);
    }

    [Fact]
    public async Task Invoke_WhenUnauthorizedAccessExceptionWithEmptyMessage_UsesDefaultDescription()
    {
        var (statusCode, body) = await InvokeMiddleware(_ =>
            throw new UnauthorizedAccessException(""));

        statusCode.Should().Be((int)HttpStatusCode.Unauthorized);

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("Description").GetString().Should().Be("Access denied.");
    }

    [Fact]
    public async Task Invoke_WhenKeyNotFoundException_Returns404()
    {
        var (statusCode, body) = await InvokeMiddleware(_ =>
            throw new KeyNotFoundException("Dealer not found"));

        statusCode.Should().Be((int)HttpStatusCode.NotFound);
        body.Should().NotBeNullOrEmpty();

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("Title").GetString().Should().Be("Not found");
        doc.RootElement.GetProperty("Description").GetString().Should().Be("Dealer not found");
        doc.RootElement.GetProperty("Status").GetInt32().Should().Be(404);
    }

    [Fact]
    public async Task Invoke_WhenKeyNotFoundExceptionWithEmptyMessage_UsesDefaultDescription()
    {
        var (statusCode, body) = await InvokeMiddleware(_ =>
            throw new KeyNotFoundException(""));

        statusCode.Should().Be((int)HttpStatusCode.NotFound);

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("Description").GetString().Should().Be("Record not found");
    }

    [Fact]
    public async Task Invoke_WhenValidationException_Returns400()
    {
        var failures = new List<ValidationFailure>
        {
            new("FirstName", "First name is required"),
            new("Email", "Email is not valid")
        };

        var (statusCode, body) = await InvokeMiddleware(_ =>
            throw new ValidationException(failures));

        statusCode.Should().Be((int)HttpStatusCode.BadRequest);
        body.Should().NotBeNullOrEmpty();

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("Title").GetString().Should().Be("Bad Request");
        doc.RootElement.GetProperty("Status").GetInt32().Should().Be(400);
    }

    [Fact]
    public async Task Invoke_WhenGenericException_Returns500()
    {
        var (statusCode, body) = await InvokeMiddleware(_ =>
            throw new Exception("Something went wrong"));

        statusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        body.Should().NotBeNullOrEmpty();

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("Title").GetString().Should().Be("Internal Server Error");
        doc.RootElement.GetProperty("Description").GetString().Should().Be("Something went wrong");
        doc.RootElement.GetProperty("Status").GetInt32().Should().Be(500);
    }

    [Fact]
    public async Task Invoke_WhenGenericExceptionWithEmptyMessage_UsesDefaultDescription()
    {
        var (statusCode, body) = await InvokeMiddleware(_ =>
            throw new Exception(""));

        statusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("Description").GetString().Should().Be("An unexpected error occurred.");
    }

    [Fact]
    public async Task Invoke_SetsContentTypeToApplicationJson_WhenExceptionOccurs()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlerMiddleware(_ => throw new KeyNotFoundException("not found"));
        await middleware.Invoke(context);

        context.Response.ContentType.Should().Be("application/json");
    }
}
