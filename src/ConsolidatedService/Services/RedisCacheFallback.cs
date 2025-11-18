namespace FluxoCaixa.Consolidated.Services
{
    public class RedisCacheFallback : IRedisCache
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, (string value, DateTime expires)> _store = new();
        public Task<string?> GetAsync(string key)
        {
            if (_store.TryGetValue(key, out var v))
            {
                if (DateTime.UtcNow <= v.expires) return Task.FromResult<string?>(v.value);
                _store.TryRemove(key, out _);
            }
            return Task.FromResult<string?>(null);
        }

        public Task SetAsync(string key, string value, TimeSpan ttl)
        {
            _store[key] = (value, DateTime.UtcNow.Add(ttl));
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _store.TryRemove(key, out _);
            return Task.CompletedTask;
        }
    }
}
