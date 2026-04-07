
namespace PoetryGlobal.Features.Poems
{
    public interface IPoemRepository
    {
        Task<List<PersistedPoemMetadata>> SearchPoemsAsync(string titleQuery, string authorQuery);
        Task<PersistedPoem?> GetPoemVersionAsync(int poemId, int languageId);

        Task<List<PersistedPoemMetadata>> SavePoemOriginalsAsync(List<Poem> originalPoemVersions);

        Task SavePoemVersionAsync(Poem poemVersion);
    }
}