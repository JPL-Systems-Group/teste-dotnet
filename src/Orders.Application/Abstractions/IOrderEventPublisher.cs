using Orders.Domain.Events;

namespace Orders.Application.Abstractions;

public interface IOrderEventPublisher
{
    Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent, CancellationToken cancellationToken = default);
}
