namespace Orders.Application.DTOs;

public sealed record OrderResponse(Guid OrderId, string CustomerId, decimal TotalAmount, DateTime CreatedAt);
