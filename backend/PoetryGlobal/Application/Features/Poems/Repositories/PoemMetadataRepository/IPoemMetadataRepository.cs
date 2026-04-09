namespace PoetryGlobal.Features.Poems
{
    /// <summary>
    /// Repository for poem metadata.
    /// </summary>
    public interface IPoemMetadataRepository
    {
        /// <summary>
        /// Performs a trigram-based search for poem metadata.
        /// </summary>
        /// <param name="titleQuery">The poem's title or a similar string.</param>
        /// <param name="authorQuery">The poem's author or a similar string.</param>
        /// <param name="limit">The maximum number of records to return.</param>
        /// <returns>A list of poem metadata.</returns>
        Task<List<PersistedPoemMetadata>> SearchAsync(string titleQuery, string authorQuery, int limit);

        /// <summary>
        /// Retrieves a poem metadata record by its ID.
        /// </summary>
        /// <param name="id">The ID of the poem metadata record.</param>
        /// <returns>A model representing the poem metadata record, or null if not found.</returns>
        Task<PersistedPoemMetadata?> GetAsync(int id);


        /// <summary>
        /// Upserts a poem metadata record.
        /// </summary>
        /// <param name="poemMetadata">The poem metadata record to upsert.</param>
        /// <returns>A model representing the upserted poem metadata record.</returns>
        /// <exception cref="ModelNotPersistableException">
        /// Thrown when a poem metadata model cannot be upserted due to it not being persistable.
        /// </exception>
        Task<PersistedPoemMetadata> UpsertAsync(PoemMetadata poemMetadata);

        /// <summary>
        /// Upserts a list of poem metadata records.
        /// </summary>
        /// <param name="poemMetadata">The list of poem metadata records to upsert.</param>
        /// <returns>A list of models representing the upserted poem metadata records.</returns>
        /// <exception cref="ModelNotPersistableException">
        /// Thrown when a poem metadata model cannot be upserted due to it not being persistable.
        /// </exception>
        Task<List<PersistedPoemMetadata>> UpsertAllAsync(List<PoemMetadata> poemMetadata);
    }
}