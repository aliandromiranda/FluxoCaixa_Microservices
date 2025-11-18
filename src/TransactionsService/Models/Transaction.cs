using System;
using System.ComponentModel.DataAnnotations;
using FluxoCaixa.Common.Enums;

namespace FluxoCaixa.Transactions.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Description { get; set; }
    }
}
