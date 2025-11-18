using FluxoCaixa.Consolidated.Data;
using FluxoCaixa.Consolidated.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FluxoCaixa.Common.Events;
using FluxoCaixa.Common.Enums;

namespace FluxoCaixa.Consolidated.Services
{
    public class ConsolidationService : IConsolidationService
    {
        private readonly ConsolidatedDbContext _db;
        public ConsolidationService(ConsolidatedDbContext db)
        {
            _db = db;
        }

        public async Task HandleTransactionCreatedAsync(string jsonEvent)
        {
            try
            {
                var evt = JsonSerializer.Deserialize<TransactionCreatedEvent>(jsonEvent);
                if (evt == null) return;
                var date = evt.Date.Date;
                var agg = await _db.DailyAggregates.FirstOrDefaultAsync(a => a.Date == date);
                var delta = evt.Type == TransactionType.Credit ? evt.Amount : -evt.Amount;
                if (agg == null)
                {
                    agg = new DailyAggregate { Date = date, Total = delta };
                    _db.DailyAggregates.Add(agg);
                }
                else
                {
                    agg.Total += delta;
                }
                await _db.SaveChangesAsync();
            }
            catch
            {
                // log in real app
            }
        }

        public async Task<IEnumerable<object>> GetConsolidatedAsync(DateTime from, DateTime to, decimal initialBalance)
        {
            var start = from.Date;
            var end = to.Date;
            var rows = await _db.DailyAggregates.Where(a => a.Date >= start && a.Date <= end).OrderBy(a => a.Date).ToListAsync();
            var list = new List<object>();
            decimal cumulative = initialBalance;
            for (var d = start; d <= end; d = d.AddDays(1))
            {
                var r = rows.FirstOrDefault(x => x.Date.Date == d);
                decimal dayTotal = r?.Total ?? 0m;
                cumulative += dayTotal;
                list.Add(new { Date = d, DayTotal = dayTotal, CumulativeBalance = cumulative });
            }
            return list;
        }
    }
}
