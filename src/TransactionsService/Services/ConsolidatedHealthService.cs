using System.Net.Http;

namespace FluxoCaixa.Transactions.Services
{
    public class ConsolidatedHealthService : IConsolidatedHealthService
    {
        private readonly HttpClient _http;

        public ConsolidatedHealthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var res = await _http.GetAsync("/health");
                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
