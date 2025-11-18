using Microsoft.EntityFrameworkCore;
using FluxoCaixa.Consolidated.Models;

namespace FluxoCaixa.Consolidated.Data
{
    public class ConsolidatedDbContext : DbContext
    {
        public ConsolidatedDbContext(DbContextOptions<ConsolidatedDbContext> options) : base(options) { }
        public DbSet<DailyAggregate> DailyAggregates { get; set; }
    }
}
