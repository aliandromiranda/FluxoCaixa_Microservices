using System;
using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Consolidated.Models
{
    public class DailyAggregate
    {
        [Key]
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }
}
