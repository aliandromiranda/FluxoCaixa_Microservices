using System;
using System.Collections.Generic;

namespace FluxoCaixa.Consolidated.Services
{
    public interface IConsolidationService
    {
        Task<IEnumerable<object>> GetConsolidatedAsync(DateTime from, DateTime to, decimal initialBalance);
        Task HandleTransactionCreatedAsync(string jsonEvent);
    }
}
