using PoetryGlobal.Exceptions;
using PoetryGlobal.Session;
using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public class ScopedPoemSearchCacheProvider(
        IPoemSearchCache poemSearchCache,
        ICurrentSession currentSession, 
        IConfiguration configuration

        ) : IScopedPoemSearchCacheProvider
    {
        private readonly ICurrentSession _currentSession = currentSession;
        private readonly IConfiguration _configuration = configuration;
        private readonly IPoemSearchCache _cache = poemSearchCache;
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

            var pageLifetimeKey = "Features:Poems:PageCacheLifetimeSeconds";
            var pageLifetimeSeconds = double.Parse(
                _configuration[pageLifetimeKey]
                    ?? throw new AppSettingsKeyNotFoundException(pageLifetimeKey)
            );

            _cache.Set(_currentSession.Guid, new SearchPages(poemMetadata, pageSize), TimeSpan.FromSeconds(pageLifetimeSeconds));
        }
    }
}