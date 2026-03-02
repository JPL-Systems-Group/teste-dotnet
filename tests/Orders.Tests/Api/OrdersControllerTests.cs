using System.Diagnostics.Metrics;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Orders.Api.Controllers;
using Orders.Application.DTOs;
using Orders.Application.Services;

namespace Orders.Tests.Api;

public sealed class OrdersControllerTests
{
    [Fact]
    public async Task CreateOrder_ShouldReturnCreated_WithLocationAndPayload()
    {
        var service = new Mock<IOrderService>();
        var response = new OrderResponse(Guid.NewGuid(), "customer-x", 77.25m, DateTime.UtcNow);
        service.Setup(s => s.CreateOrderAsync(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        using var meter = new Meter("Orders.Api.Tests");
        var controller = new OrdersController(service.Object, meter);

        var result = await controller.CreateOrder(new CreateOrderRequest("customer-x", 77.25m), CancellationToken.None);

        var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Location.Should().Be($"/orders/{response.OrderId}");
        createdResult.Value.Should().BeEquivalentTo(response);
    }
}
