using System.Collections.Concurrent;

namespace PoetryGlobal.Shared.MemoryCache
{
    public class MemoryCache<T> : IMemoryCache<T>
    {
        private readonly ConcurrentDictionary<string, (T Value, DateTime? Expiration)> _cache = new();

        public T? Get(string key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.Expiration == null || entry.Expiration > DateTime.UtcNow)
                {
                    return entry.Value;
                }
                else {
                    var keyValuePair = new KeyValuePair<string, (T Value, DateTime? Expiration)>
                    (
                        key, 
                        entry
                    );
                    _cache.TryRemove(keyValuePair);
                }
            }
            return default;
            
        }

        public void Set(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var expiration = absoluteExpirationRelativeToNow.HasValue ? DateTime.UtcNow.Add(absoluteExpirationRelativeToNow.Value) : (DateTime?)null;
            _cache[key] = (value, expiration);
        }

        public bool Remove(string key)
        {
            return _cache.TryRemove(key, out _);
        }
    }
}