using System;
using FluxoCaixa.Common.Enums;

namespace FluxoCaixa.Common.Events
{
    public class TransactionCreatedEvent
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Description { get; set; }
    }
}
