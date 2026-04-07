
using PoetryGlobal.Shared.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public interface IPoemSearchCache
    {
        ISearchPages? GetSearchCache();
        void SetSearchCache(List<PersistedPoemMetadata> poemMetadata);
    }

    public interface ISearchPages
    {
        PersistedPoemMetadata[] GetPage(int page);
    }

}