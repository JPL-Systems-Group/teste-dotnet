using Orders.Domain.Exceptions;

namespace Orders.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order(Guid id, string customerId, decimal totalAmount, DateTime createdAt)
    {
        Id = id;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        CreatedAt = createdAt;
    }

    public static Order Create(string customerId, decimal totalAmount)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new DomainValidationException("customerId is required");
        }

        if (totalAmount <= 0)
        {
            throw new DomainValidationException("totalAmount must be greater than zero");
        }

        return new Order(Guid.NewGuid(), customerId.Trim(), totalAmount, DateTime.UtcNow);
    }
}
