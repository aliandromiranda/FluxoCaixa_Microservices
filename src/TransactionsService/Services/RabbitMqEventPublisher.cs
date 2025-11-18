using FluxoCaixa.Common.Events;
using FluxoCaixa.Transactions.Services;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;

    public RabbitMqEventPublisher(IConfiguration config)
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(config["RabbitMq:ConnectionString"]!)
        };

        _connection = factory.CreateConnection();
    }

    public Task PublishAsync(TransactionCreatedEvent evt)
    {
        using var channel = _connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: "transactions",
            type: ExchangeType.Fanout,
            durable: true
        );

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));

        channel.BasicPublish(
            exchange: "transactions",
            routingKey: "",
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }
}
