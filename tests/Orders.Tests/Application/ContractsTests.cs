using FluentAssertions;
using Orders.Application.DTOs;

namespace Orders.Tests.Application;

public sealed class ContractsTests
{
    [Fact]
    public void CreateOrderRequest_ShouldKeepValues()
    {
        var request = new CreateOrderRequest("c-1", 10.5m);

        request.CustomerId.Should().Be("c-1");
        request.TotalAmount.Should().Be(10.5m);
    }

    [Fact]
    public void OrderResponse_ShouldKeepValues()
    {
        var now = DateTime.UtcNow;
        var response = new OrderResponse(Guid.NewGuid(), "c-2", 11, now);

        response.CustomerId.Should().Be("c-2");
        response.TotalAmount.Should().Be(11);
        response.CreatedAt.Should().Be(now);
    }
}
