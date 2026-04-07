using System.Collections.Concurrent;

namespace PoetryGlobal.Shared.SimpleCache
{
    public class SimpleCache<K, V> : ISimpleCache<K, V> where K : notnull
    {
        private readonly ConcurrentDictionary<K, (V Value, DateTime? Expiration)> _cache = new();

        public V? Get(K key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.Expiration == null || entry.Expiration > DateTime.UtcNow)
                {
                    return entry.Value;
                }
                else {
                    var keyValuePair = new KeyValuePair<K, (V Value, DateTime? Expiration)>
                    (
                        key, 
                        entry
                    );
                    _cache.TryRemove(keyValuePair);
                }
            }
            return default;
            
        }

        public void Set(K key, V value, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var expiration = absoluteExpirationRelativeToNow.HasValue ? DateTime.UtcNow.Add(absoluteExpirationRelativeToNow.Value) : (DateTime?)null;
            _cache[key] = (value, expiration);
        }

        public bool Remove(K key)
        {
            return _cache.TryRemove(key, out _);
        }
    }
}