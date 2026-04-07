using PoetryGlobal.Exceptions;
using PoetryGlobal.GlobalData.Session;
using PoetryGlobal.Shared.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public class PoemSearchCache(CurrentSession currentSession, IConfiguration configuration) : IPoemSearchCache
    {
        private readonly CurrentSession _currentSession = currentSession;
        private readonly IConfiguration _configuration = configuration;
        private static readonly ISimpleCache<Guid, ISearchPages> _cache = new SimpleCache<Guid, ISearchPages>();
        public ISearchPages? GetSearchCache()
        {
            return _cache.Get(_currentSession.Guid);
        }

        public void SetSearchCache(List<PersistedPoemMetadata> poemMetadata)
        {
            var pageSizeKey = "Features:Poems:PageSize";
            var pageSize = int.Parse(
                _configuration[pageSizeKey]
                    ?? throw new AppSettingsKeyNotFoundException(pageSizeKey)
            );

            _cache.Set(_currentSession.Guid, new SearchPages(poemMetadata, pageSize));
        }
    }

    public class SearchPages(List<PersistedPoemMetadata> poemMetadata, int pageSize) : ISearchPages
    {
        private readonly PersistedPoemMetadata[][] _pages = [.. poemMetadata.Chunk(pageSize)];

        public PersistedPoemMetadata[] GetPage(int page)
        {
            return _pages[page];
        }
    }

}