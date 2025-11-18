using Microsoft.Extensions.Hosting;

namespace FluxoCaixa.Consolidated.Services
{
    public class RabbitBackgroundService : BackgroundService
    {
        private readonly IEventConsumer _consumer;
        private readonly IServiceProvider _serviceProvider;

        public RabbitBackgroundService(IEventConsumer consumer, IServiceProvider serviceProvider)
        {
            _consumer = consumer;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Start(async (jsonEvent) =>
            {
                // Criar escopo correto dentro do singleton
                using var scope = _serviceProvider.CreateScope();

                var consolidator =
                    scope.ServiceProvider.GetRequiredService<IConsolidationService>();

                await consolidator.HandleTransactionCreatedAsync(jsonEvent);
            });

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Stop();
            return base.StopAsync(cancellationToken);
        }
    }
}
