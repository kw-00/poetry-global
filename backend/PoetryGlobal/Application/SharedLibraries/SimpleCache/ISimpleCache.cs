namespace PoetryGlobal.Shared.SimpleCache
{
    public interface ISimpleCache<K, V>
    {
        V? Get(K key);
        void Set(K key, V value, TimeSpan? absoluteExpirationRelativeToNow = null);
        bool Remove(K key);
    }
}