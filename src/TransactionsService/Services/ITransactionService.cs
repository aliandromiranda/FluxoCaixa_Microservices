using FluxoCaixa.Common.Dtos;
using FluxoCaixa.Transactions.Models;

namespace FluxoCaixa.Transactions.Services
{
    public interface ITransactionService
    {
        Task<Transaction> AddAsync(FluxoCaixa.Common.Dtos.TransactionDto dto);
    }
}
