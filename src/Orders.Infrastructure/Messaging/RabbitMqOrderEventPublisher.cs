using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orders.Application.Abstractions;
using Orders.Domain.Events;
using RabbitMQ.Client;

namespace Orders.Infrastructure.Messaging;

public sealed class RabbitMqOrderEventPublisher(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqOrderEventPublisher> logger) : IOrderEventPublisher
{
    public Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent, CancellationToken cancellationToken = default)
    {
        var rabbitOptions = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = rabbitOptions.HostName,
            UserName = rabbitOptions.UserName,
            Password = rabbitOptions.Password,
            Port = rabbitOptions.Port
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: rabbitOptions.ExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);

        var payload = JsonSerializer.Serialize(orderCreatedEvent);
        var body = Encoding.UTF8.GetBytes(payload);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(
            exchange: rabbitOptions.ExchangeName,
            routingKey: string.Empty,
            basicProperties: properties,
            body: body);

        logger.LogInformation(
            "OrderCreated event published. OrderId={OrderId}, CustomerId={CustomerId}, TotalAmount={TotalAmount}, CreatedAt={CreatedAt}",
            orderCreatedEvent.OrderId,
            orderCreatedEvent.CustomerId,
            orderCreatedEvent.TotalAmount,
            orderCreatedEvent.CreatedAt);

        return Task.CompletedTask;
    }
}
