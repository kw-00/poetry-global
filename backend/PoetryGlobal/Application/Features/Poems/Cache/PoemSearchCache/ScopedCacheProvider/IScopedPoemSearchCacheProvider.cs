
using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public interface IScopedPoemSearchCacheProvider
    {
        ISearchPages? GetSearchCache();
        void SetSearchCache(List<PersistedPoemMetadata> poemMetadata);
    }

    public class SearchPages(List<PersistedPoemMetadata> poemMetadata, int pageSize) : ISearchPages
    {
        private readonly PersistedPoemMetadata[][] _pages = [.. poemMetadata.Chunk(pageSize)];

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