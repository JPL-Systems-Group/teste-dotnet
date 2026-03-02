using FluentAssertions;
using Orders.Domain.Entities;
using Orders.Domain.Exceptions;

namespace Orders.Tests.Domain;

public sealed class OrderTests
{
    [Fact]
    public void Create_ShouldCreateOrder_WhenPayloadIsValid()
    {
        var order = Order.Create("customer-1", 100);

        order.Id.Should().NotBeEmpty();
        order.CustomerId.Should().Be("customer-1");
        order.TotalAmount.Should().Be(100);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_ShouldThrow_WhenAmountIsInvalid()
    {
        var act = () => Order.Create("customer-1", 0);

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*totalAmount*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenCustomerIsMissing()
    {
        var act = () => Order.Create("", 10);

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*customerId*");
    }
}
