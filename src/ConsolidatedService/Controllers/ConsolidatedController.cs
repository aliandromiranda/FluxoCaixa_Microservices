using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Consolidated.Services;

namespace FluxoCaixa.Consolidated.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsolidatedController : ControllerBase
    {
        private readonly IConsolidationService _service;
        private readonly IRedisCache _cache;
        public ConsolidatedController(IConsolidationService service, IRedisCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpGet("/health")]
        public IActionResult Health() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] decimal initialBalance = 0)
        {
            var cacheKey = $"consolidated:{from:yyyy-MM-dd}:{to:yyyy-MM-dd}:{initialBalance}";
            var cached = await _cache.GetAsync(cacheKey);
            if (cached != null) return Content(cached, "application/json");

            var result = await _service.GetConsolidatedAsync(from, to, initialBalance);
            var json = System.Text.Json.JsonSerializer.Serialize(result);
            await _cache.SetAsync(cacheKey, json, TimeSpan.FromSeconds(30));
            return Content(json, "application/json");
        }
    }
}
