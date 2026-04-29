
namespace PoetryGlobal.Infrastructure
{
    /// <summary>
    /// Repository for poem versions.
    /// </summary>
    public interface IPoemVersionRepository
    {
        /// <summary>
        /// Retrieves a poem version from the database.
        /// </summary>
        /// <returns></returns>
        Task<PersistedPoemVersion?> GetAsync(int poemVersionId, int languageId);

        /// <summary>
        /// Retrieves the original version of a poem from the database.
        /// </summary>
        /// <returns>A model representing the original version of the poem, or null if not found.</returns>
        Task<PersistedPoemVersion?> GetOriginalAsync(int poemVersionId);

        /// <summary>
        /// Upserts a poem version into the database.
        /// </summary>
        /// <param name="poemVersion">The poem version to upsert.</param>
        /// <returns>A model representing the upserted poem version.</returns>
        Task<PersistedPoemVersion> UpsertAsync(PoemVersion poemVersion);
        

        /// <summary>
        /// Upserts a list of poem versions into the database.
        /// </summary>
        /// <param name="originalPoemVersions">The list of poem versions to upsert.</param>
        /// <returns>A list of models representing the upserted poem versions.</returns>
        Task<List<PersistedPoemVersion>> UpsertAllAsync(List<PoemVersion> originalPoemVersions);

    }
}