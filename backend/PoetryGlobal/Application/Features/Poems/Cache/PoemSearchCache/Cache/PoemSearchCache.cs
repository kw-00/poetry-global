using PoetryGlobal.Exceptions;
using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public class PoemSearchCache : SimpleCache<Guid, ISearchPages>, IPoemSearchCache
    {
        private static readonly string _CLEANUP_INTERVAL_CONFIG_KEY = "Features:Poems:PageCacheCleanupIntervalSeconds";

        public PoemSearchCache(IConfiguration configuration) : base(int.MaxValue)
        {
            var cleanupIntervalSeconds = int.Parse(configuration[_CLEANUP_INTERVAL_CONFIG_KEY] 
                ?? throw new AppSettingsKeyNotFoundException(_CLEANUP_INTERVAL_CONFIG_KEY));

            _timer.Stop();
            _timer.Interval = cleanupIntervalSeconds * 1000;
            _timer.Start();
        }
    }
}