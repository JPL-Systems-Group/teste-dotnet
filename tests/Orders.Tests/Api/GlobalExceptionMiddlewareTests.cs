using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Orders.Api.Middleware;
using Orders.Domain.Exceptions;

namespace Orders.Tests.Api;

public sealed class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturnBadRequest_WhenValidationExceptionOccurs()
    {
        RequestDelegate next = _ => throw new DomainValidationException("invalid");
        var middleware = new GlobalExceptionMiddleware(next, Mock.Of<ILogger<GlobalExceptionMiddleware>>());
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        context.Response.Body.Position = 0;
        var payload = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);
        payload.GetProperty("error").GetString().Should().Be("invalid");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnInternalServerError_WhenUnhandledExceptionOccurs()
    {
        RequestDelegate next = _ => throw new Exception("boom");
        var middleware = new GlobalExceptionMiddleware(next, Mock.Of<ILogger<GlobalExceptionMiddleware>>());
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
