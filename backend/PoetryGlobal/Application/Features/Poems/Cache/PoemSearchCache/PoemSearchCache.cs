using PoetryGlobal.SharedDIDependencies;
using PoetryGlobal.Exceptions;
using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Features.Poems
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

    public class SearchPages(List<PersistedPoemMetadata> poemMetadata, int pageSize) : ISearchPages
    {
        private readonly PersistedPoemMetadata[][] _pages = [.. poemMetadata.Chunk(pageSize)];

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