using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace FluxoCaixa.Consolidated.Services
{
    public class RabbitMqEventConsumer : IEventConsumer
    {
        private readonly IConfiguration _config;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqEventConsumer(IConfiguration config)
        {
            _config = config;
        }

        public void Start(Func<string, Task> handler)
        {
            try
            {
                var factory = new ConnectionFactory() { Uri = new Uri(_config.GetValue<string>("RabbitMq:ConnectionString") ?? "amqp://guest:guest@localhost:5672/") };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "transactions", type: ExchangeType.Fanout, durable: true);
                var queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: queueName, exchange: "transactions", routingKey: "");

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    await handler(json);
                };
                _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            }
            catch
            {
                // ignore for fallback
            }
        }

        public void Stop()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch { }
        }
    }
}
