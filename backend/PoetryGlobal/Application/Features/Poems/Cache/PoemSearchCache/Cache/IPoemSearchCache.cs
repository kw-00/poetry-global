using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public interface IPoemSearchCache : ISimpleCache<Guid, ISearchPages>;

    public interface ISearchPages
    {
        PersistedPoemMetadata[]? GetPage(int page);
    }
}