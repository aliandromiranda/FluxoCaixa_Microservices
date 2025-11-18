using FluxoCaixa.Transactions.Data;
using FluxoCaixa.Transactions.Models;
using FluxoCaixa.Common.Dtos;
using FluxoCaixa.Common.Events;

namespace FluxoCaixa.Transactions.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionsDbContext _db;
        private readonly IConsolidatedHealthService _health;
        private readonly IEventPublisher _publisher;
        private readonly IConfiguration _config;

        public TransactionService(TransactionsDbContext db, IConsolidatedHealthService health, IEventPublisher publisher, IConfiguration config)
        {
            _db = db;
            _health = health;
            _publisher = publisher;
            _config = config;
        }

        public async Task<Transaction> AddAsync(TransactionDto dto)
        {
            var block = _config.GetValue<bool>("ServiceOptions:BlockOnConsolidatedDown", true);
            var healthy = await _health.IsAvailableAsync();
            if (!healthy && block)
            {
                throw new InvalidOperationException("Serviço de consolidado indisponível. Tente novamente mais tarde.");
            }

            var entity = new Transaction
            {
                Id = Guid.NewGuid(),
                Date = dto.Date,
                Amount = dto.Amount,
                Type = dto.Type,
                Description = dto.Description
            };
            _db.Transactions.Add(entity);
            await _db.SaveChangesAsync();

            // publish event (best-effort)
            try
            {
                var ev = new TransactionCreatedEvent { Id = entity.Id, Date = entity.Date, Amount = entity.Amount, Type = entity.Type, Description = entity.Description };
                await _publisher.PublishAsync(ev);
            }
            catch
            {
                // swallow - keep availability
            }

            return entity;
        }
    }
}
