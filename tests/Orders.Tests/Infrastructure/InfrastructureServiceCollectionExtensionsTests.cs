using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Domain.Repositories;
using Orders.Infrastructure.DependencyInjection;
using Orders.Infrastructure.Messaging;
using Orders.Infrastructure.Persistence;

namespace Orders.Tests.Infrastructure;

public sealed class InfrastructureServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructure_ShouldRegisterDbContextRepositoryAndPublisher()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:OrdersDb"] = "Data Source=:memory:",
                ["RabbitMq:HostName"] = "rabbitmq"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(config);

        using var provider = services.BuildServiceProvider();

        provider.GetService<OrdersDbContext>().Should().NotBeNull();
        provider.GetService<IOrderRepository>().Should().NotBeNull();
        provider.GetService<IOrderEventPublisher>().Should().BeOfType<RabbitMqOrderEventPublisher>();
    }
}
