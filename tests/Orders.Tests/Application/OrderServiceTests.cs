using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Orders.Application.Abstractions;
using Orders.Application.DTOs;
using Orders.Application.Services;
using Orders.Domain.Entities;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

namespace Orders.Tests.Application;

public sealed class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IOrderEventPublisher> _eventPublisher = new();
    private readonly Mock<ILogger<OrderService>> _logger = new();

    [Fact]
    public async Task CreateOrderAsync_ShouldPersistAndPublish()
    {
        var service = new OrderService(_orderRepository.Object, _eventPublisher.Object, _logger.Object);
        var request = new CreateOrderRequest("customer-2", 55.50m);

        var result = await service.CreateOrderAsync(request);

        _orderRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisher.Verify(p => p.PublishOrderCreatedAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        result.CustomerId.Should().Be("customer-2");
        result.TotalAmount.Should().Be(55.50m);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrow_WhenPublisherFails()
    {
        _eventPublisher
            .Setup(p => p.PublishOrderCreatedAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("RabbitMQ unavailable"));

        var service = new OrderService(_orderRepository.Object, _eventPublisher.Object, _logger.Object);

        var act = async () => await service.CreateOrderAsync(new CreateOrderRequest("customer-2", 10));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
