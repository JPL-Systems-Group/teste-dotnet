using Microsoft.Extensions.Logging;
using Orders.Application.Abstractions;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

namespace Orders.Application.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IOrderEventPublisher orderEventPublisher,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = Order.Create(request.CustomerId, request.TotalAmount);

        await orderRepository.AddAsync(order, cancellationToken);

        var orderCreatedEvent = new OrderCreatedEvent(order.Id, order.CustomerId, order.TotalAmount, order.CreatedAt);

        try
        {
            await orderEventPublisher.PublishOrderCreatedAsync(orderCreatedEvent, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to publish OrderCreated event for order {OrderId}. Data consistency must be recovered asynchronously.",
                order.Id);
            throw;
        }

        logger.LogInformation("Order {OrderId} created for customer {CustomerId} with total {TotalAmount}",
            order.Id,
            order.CustomerId,
            order.TotalAmount);

        return new OrderResponse(order.Id, order.CustomerId, order.TotalAmount, order.CreatedAt);
    }
}
