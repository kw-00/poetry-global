
namespace PoetryGlobal.Features.Poems
{
    public interface IPoemVersionRepository
    {
        Task<PersistedPoemVersion?> GetAsync(int poemMetadataId, int languageId);

        Task<PersistedPoemVersion?> GetOriginalAsync(int poemMetadataId);

        Task<PersistedPoemVersion> UpsertAsync(PoemVersion poemVersion);
        
        Task<List<PersistedPoemVersion>> UpsertAllAsync(List<PoemVersion> originalPoemVersions);

    }
}