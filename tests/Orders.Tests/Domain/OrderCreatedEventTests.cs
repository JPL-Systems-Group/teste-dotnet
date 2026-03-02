using FluentAssertions;
using Orders.Domain.Events;

namespace Orders.Tests.Domain;

public sealed class OrderCreatedEventTests
{
    [Fact]
    public void Event_ShouldExposeAllRequiredFields()
    {
        var now = DateTime.UtcNow;
        var evt = new OrderCreatedEvent(Guid.NewGuid(), "customer-7", 999.99m, now);

        evt.CustomerId.Should().Be("customer-7");
        evt.TotalAmount.Should().Be(999.99m);
        evt.CreatedAt.Should().Be(now);
        evt.OrderId.Should().NotBeEmpty();
    }
}
