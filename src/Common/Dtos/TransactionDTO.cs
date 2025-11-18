using System;
using FluxoCaixa.Common.Enums;

namespace FluxoCaixa.Common.Dtos
{
    public class TransactionDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Description { get; set; }
    }
}
