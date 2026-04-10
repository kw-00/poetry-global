namespace PoetryGlobal.SimpleCache
{
    public interface ISimpleCache<K, V>
    {
        TimeSpan? SlidingItemLifetime { get; set; }
        TimeSpan? AbsoluteItemLifetime { get; set; }
        V? Get(K key);
        void Set(K key, V value);
    }
}