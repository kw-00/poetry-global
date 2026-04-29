using PoetryGlobal.Config;
using PoetryGlobal.Infrastructure;
using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Infrastructure
{
    public class PoemSearchCache : SimpleCache<SearchQueryDTO, ISearchPages>, IPoemSearchCache
    {
        public PoemSearchCache(IConfigWithValidation configuration) : base()
        {
            var slidingLifetime = configuration.GetFromConfigOrThrow<int>(
                "Features:Poems:Search:PageCacheSlidingLifetimeSeconds"
            );
            var absoluteLifetime = configuration.GetFromConfigOrThrow<int>(
                "Features:Poems:Search:PageCacheAbsoluteLifetimeMinutes"
            );

            SlidingItemLifetime = TimeSpan.FromSeconds(slidingLifetime);
            AbsoluteItemLifetime = TimeSpan.FromMinutes(absoluteLifetime);
        }
    }

    public class SearchPages(IReadOnlyList<PersistedPoemMetadata> poemMetadata, int pageSize) : ISearchPages
    {
        private readonly PersistedPoemMetadata[][] _pages = [.. poemMetadata.Chunk(pageSize)];

        public int PageCount => _pages.Length;

        public IReadOnlyList<PersistedPoemMetadata>? GetPage(int page)
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