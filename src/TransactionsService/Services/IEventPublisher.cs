using FluxoCaixa.Common.Events;

namespace FluxoCaixa.Transactions.Services
{
    public interface IEventPublisher
    {
        Task PublishAsync(TransactionCreatedEvent evt);
    }
}
