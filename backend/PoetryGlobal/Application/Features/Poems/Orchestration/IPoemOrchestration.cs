
namespace PoetryGlobal.Features.Poems
{
    public interface IPoemOrchestration
    {
        Task<List<PersistedPoemMetadata>> SearchPoemsAsync(string titleQuery, string authorQuery);
        Task<PersistedPoem> GetPoemAsync(int poemId, int languageId);
    }
}