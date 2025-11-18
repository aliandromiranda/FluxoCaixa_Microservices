using Microsoft.EntityFrameworkCore;
using FluxoCaixa.Transactions.Models;

namespace FluxoCaixa.Transactions.Data
{
    public class TransactionsDbContext : DbContext
    {
        public TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : base(options) { }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
