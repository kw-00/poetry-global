namespace PoetryGlobal.Features.Poems
{
    public interface IPoemMetadataRepository
    {
        Task<List<PersistedPoemMetadata>> SearchAsync(string titleQuery, string authorQuery, int limit);

        Task<PersistedPoemMetadata?> GetAsync(int id);

        Task<PersistedPoemMetadata> UpsertAsync(PoemMetadata poemMetadata);

        Task<List<PersistedPoemMetadata>> UpsertAllAsync(List<PoemMetadata> poemMetadata);
    }
}