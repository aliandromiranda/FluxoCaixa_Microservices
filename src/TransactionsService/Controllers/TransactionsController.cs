using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Transactions.Services;
using FluxoCaixa.Common.Dtos;

namespace FluxoCaixa.Transactions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TransactionDto dto)
        {
            if (dto.Amount <= 0) return BadRequest("Amount must be > 0");
            try
            {
                var created = await _service.AddAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(503, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Get() => Ok(new { message = "Transactions service is running" });
    }
}
