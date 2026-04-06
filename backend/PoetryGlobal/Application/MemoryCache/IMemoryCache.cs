namespace PoetryGlobal.MemoryCache
{
    public interface IMemoryCache<T>
    {
        T? Get(string key);
        void Set(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);
        bool Remove(string key);
    }
}