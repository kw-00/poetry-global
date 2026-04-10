using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace PoetryGlobal.SimpleCache
{
    public class SimpleCache<K, V> : ISimpleCache<K, V>, IDisposable where K : notnull
    {
        public TimeSpan? SlidingItemLifetime { get; set; }
        public TimeSpan? AbsoluteItemLifetime { get; set; }
        protected readonly MemoryCache _cache;


        public SimpleCache(
            MemoryCacheOptions? memoryCacheOptions = null, 
            TimeSpan? slidingItemLifetime = null, 
            TimeSpan? absoluteItemLifetime = null
        )
        {
            if (memoryCacheOptions is null) memoryCacheOptions = new MemoryCacheOptions();
            _cache = new MemoryCache(memoryCacheOptions);

            SlidingItemLifetime = slidingItemLifetime;
            AbsoluteItemLifetime = absoluteItemLifetime;
        }

        public V? Get(K key)
        {
            return _cache.Get<V>(key);
        }

        public void Set(K key, V value)
        {
            var entryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = SlidingItemLifetime,
                AbsoluteExpiration = DateTimeOffset.UtcNow + AbsoluteItemLifetime
            };
            _cache.Set(key, value, entryOptions);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _cache.Dispose();
        }
    }
}