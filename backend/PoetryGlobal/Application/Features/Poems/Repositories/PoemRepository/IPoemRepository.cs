
namespace PoetryGlobal.Features.Poems
{
    public interface IPoemRepository
    {
        Task<List<PersistedPoemMetadata>> SearchPoemsAsync(string titleQuery, string authorQuery);

        Task<PersistedPoem?> GetPoemOriginalAsync(int poemId);

        Task<PersistedPoem?> GetPoemVersionAsync(int poemId, int languageId);

        Task SavePoemOriginalsAsync(List<Poem> originalPoemVersions);

        Task<PersistedPoem?> SavePoemVersionAsync(Poem poemVersion);
    }
}