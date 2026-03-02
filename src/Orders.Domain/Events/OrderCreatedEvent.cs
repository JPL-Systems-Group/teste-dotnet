namespace Orders.Domain.Events;

public sealed record OrderCreatedEvent(
    Guid OrderId,
    string CustomerId,
    decimal TotalAmount,
    DateTime CreatedAt);
