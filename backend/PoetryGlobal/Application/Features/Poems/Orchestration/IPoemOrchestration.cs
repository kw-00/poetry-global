
namespace PoetryGlobal.Features.Poems
{
    public interface IPoemOrchestration
    {
        Task PreparePagesAsync(string titleQuery, string authorQuery);
        PersistedPoemMetadata[] GetPage(int page);
        Task<PoemDTO> GetPoemAsync(int poemId, int languageId);
    }
}