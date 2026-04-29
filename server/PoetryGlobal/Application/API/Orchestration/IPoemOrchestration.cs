
using PoetryGlobal.Infrastructure;

namespace PoetryGlobal.API
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
        /// Performs a search for poem metadata and stores the results in the cache.
        /// Then returns the number of pages and a chosen page or null if it cannot be found.
        /// 
        /// If the search query has already been cached, it uses the existing cache.
        /// </summary>
        /// <param name="query">Object representation of the search query.</param>
        /// <param name="pageNumber">Zero-indexed page number.</param>
        /// <returns>The number of pages and a chosen page.</returns>
        Task<(int PageCount, IReadOnlyList<PersistedPoemMetadata> Page)> SearchAsync(
            SearchQueryDTO query, int pageNumber
        );

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