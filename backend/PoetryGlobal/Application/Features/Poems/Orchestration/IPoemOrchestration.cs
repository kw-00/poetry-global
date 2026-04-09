
namespace PoetryGlobal.Features.Poems
{
    /// <summary>
    /// Service orchestration layer for poems. Exposes methods that
    /// orchestrate service layer methods to provide higher-level functionality.
    /// 
    /// Use directly in controllers.
    /// </summary>
    public interface IPoemOrchestration
    {
        /// <summary>
        /// Prepares pages of poem metadata based on a search query
        /// and caches them in application memory for later retrieval.
        /// Cached pages may expire after.
        /// </summary>
        /// <param name="titleQuery">Poem title or part of title.</param>
        /// <param name="authorQuery">Poem author or part of author name.</param>
        /// <returns>The number of pages cached.</returns>
        Task<int> PreparePagesAsync(string titleQuery, string authorQuery);

        /// <summary>
        /// Retrieves a page of poem metadata from the cache.
        /// </summary>
        /// <param name="pageNumber">Zero-indexed page number.</param>
        /// <returns>An array of poem metadata corresponding to <paramref name="pageNumber"/>.</returns>
        PersistedPoemMetadata[] GetPage(int pageNumber);

        /// <summary>
        /// Retrieves a poem version in the specified language from the database or attempts
        /// to translate it from the original version.
        /// 
        /// In case the poem's original language matches <paramref name="languageId"/>,
        /// the original version is returned.
        /// </summary>
        /// <param name="poemId">The ID of the poem.</param>
        /// <param name="languageId">The ID of the target language.</param>
        /// <returns>A DTO representing the resulting poem version.</returns>
        /// <exception cref="Exceptions.NotFoundException">
        /// When the <paramref name="poemId"/> or <paramref name="languageId"/> does not exist in the database,
        /// or original poem version cannot be found,
        /// or original version's language does not exist in the database.
        /// </exception>
        Task<PoemDTO> GetPoemAsync(int poemId, int languageId);
    }
}