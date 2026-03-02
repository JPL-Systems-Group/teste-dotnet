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
        var maxAttempts = Math.Max(1, rabbitOptions.PublishRetryCount);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                PublishMessage(orderCreatedEvent, rabbitOptions);
                logger.LogInformation(
                    "OrderCreated event published. OrderId={OrderId}, CustomerId={CustomerId}, TotalAmount={TotalAmount}, CreatedAt={CreatedAt}, Attempt={Attempt}",
                    orderCreatedEvent.OrderId,
                    orderCreatedEvent.CustomerId,
                    orderCreatedEvent.TotalAmount,
                    orderCreatedEvent.CreatedAt,
                    attempt);
                return Task.CompletedTask;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                var delay = TimeSpan.FromMilliseconds(Math.Max(50, rabbitOptions.PublishRetryDelayMs) * attempt);
                logger.LogWarning(ex,
                    "Failed to publish OrderCreated event for order {OrderId} on attempt {Attempt}/{MaxAttempts}. Retrying in {DelayMs}ms.",
                    orderCreatedEvent.OrderId,
                    attempt,
                    maxAttempts,
                    delay.TotalMilliseconds);
                Task.Delay(delay, cancellationToken).GetAwaiter().GetResult();
            }
        }

        PublishMessage(orderCreatedEvent, rabbitOptions);
        return Task.CompletedTask;
    }

    private static void PublishMessage(OrderCreatedEvent orderCreatedEvent, RabbitMqOptions rabbitOptions)
    {
        var factory = new ConnectionFactory
        {
            HostName = rabbitOptions.HostName,
            UserName = rabbitOptions.UserName,
            Password = rabbitOptions.Password,
            Port = rabbitOptions.Port,
            DispatchConsumersAsync = true
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: rabbitOptions.ExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);

        var payload = JsonSerializer.Serialize(orderCreatedEvent);
        var body = Encoding.UTF8.GetBytes(payload);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        channel.BasicPublish(
            exchange: rabbitOptions.ExchangeName,
            routingKey: string.Empty,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}
