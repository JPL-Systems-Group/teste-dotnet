namespace Orders.Application.DTOs;

public sealed record CreateOrderRequest(string CustomerId, decimal TotalAmount);
