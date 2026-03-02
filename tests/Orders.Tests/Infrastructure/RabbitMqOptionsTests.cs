using FluentAssertions;
using Orders.Infrastructure.Messaging;

namespace Orders.Tests.Infrastructure;

public sealed class RabbitMqOptionsTests
{
    [Fact]
    public void Defaults_ShouldMatchLocalDevelopment()
    {
        var options = new RabbitMqOptions();

        options.HostName.Should().Be("localhost");
        options.Port.Should().Be(5672);
        options.UserName.Should().Be("guest");
        options.Password.Should().Be("guest");
        options.ExchangeName.Should().Be("orders.events");
    }
}
