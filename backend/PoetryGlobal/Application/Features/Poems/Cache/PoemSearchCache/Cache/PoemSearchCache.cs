using PoetryGlobal.Exceptions;
using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public class PoemSearchCache : SimpleCache<Guid, ISearchPages>, IPoemSearchCache
    {
        private static readonly string _cleanupIntervalConfigKey = "Features:Poems:PageCacheCleanupIntervalSeconds";

        public PoemSearchCache(IConfiguration configuration) : base(int.MaxValue)
        {
            var cleanupIntervalSeconds = int.Parse(configuration[_cleanupIntervalConfigKey] 
                ?? throw new AppSettingsKeyNotFoundException(_cleanupIntervalConfigKey));

            _timer.Stop();
            _timer.Interval = cleanupIntervalSeconds * 1000;
            _timer.Start();
        }
    }

    public class SearchPages(List<PersistedPoemMetadata> poemMetadata, int pageSize) : ISearchPages
    {
        private readonly PersistedPoemMetadata[][] _pages = [.. poemMetadata.Chunk(pageSize)];

        static IList<string> list = [];

        public int PageCount => _pages.Length;

        public PersistedPoemMetadata[]? GetPage(int page)
        {
            try
            {
                return _pages[page];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }
    }
}