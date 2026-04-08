using System.Collections.Concurrent;

namespace PoetryGlobal.SimpleCache
{
    public class SimpleCache<K, V> : ISimpleCache<K, V>, IDisposable where K : notnull
    {
        protected volatile bool _disposed = false;
        protected readonly System.Timers.Timer _timer;
        protected readonly ConcurrentDictionary<K, (V Value, DateTime? Expiration)> _cache = new();


        public SimpleCache(double cleanupIntervalMilis)
        {
            _timer = new System.Timers.Timer(cleanupIntervalMilis);
            _timer.Elapsed += (sender, e) => RemoveExpiredItems();
            _timer.AutoReset = true;
            _timer.Start();
        }

        public V? Get(K key)
        {
            CheckDisposed();
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
            CheckDisposed();
            var expiration = absoluteExpirationRelativeToNow.HasValue 
                ? DateTime.UtcNow.Add(absoluteExpirationRelativeToNow.Value) 
                : (DateTime?)null;

            _cache[key] = (value, expiration);
        }

        public bool Remove(K key)
        {
            CheckDisposed();
            return _cache.TryRemove(key, out _);
        }

        public void Dispose()
        {
            _disposed = true;
            _timer.Dispose();
            _cache.Clear();
            GC.SuppressFinalize(this);
        }

        void CheckDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }

        private void RemoveExpiredItems()
        {
            foreach (var key in _cache.Keys)
            {
                if (_cache[key].Expiration < DateTime.UtcNow)
                {
                    _cache.TryRemove(key, out _);
                }
            }
        }
    }
}