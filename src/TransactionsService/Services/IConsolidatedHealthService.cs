namespace FluxoCaixa.Transactions.Services
{
    public interface IConsolidatedHealthService
    {
        Task<bool> IsAvailableAsync();
    }
}
