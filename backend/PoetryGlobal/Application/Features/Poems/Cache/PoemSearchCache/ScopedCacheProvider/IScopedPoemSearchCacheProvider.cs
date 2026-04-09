
namespace PoetryGlobal.Features.Poems
{
    /// <summary>
    /// A wrapper for IPoemSearchCache designed to provide a more abstract,
    /// use-case specific form of access.
    /// 
    /// Most importantly, it takes into account the current <see cref="ISearchPages"/> HttpContext session and only accesses
    /// data that belongs to that session.
    /// </summary>
    public interface IScopedPoemSearchCacheProvider
    {
        /// <summary>
        /// Gets the search cache for the current <see cref="HttpContext"/> HttpContext session.
        /// </summary>
        /// <returns>Cached search query pages for current <see cref="HttpContext"/> session or null if session is invalid.</returns>
        ISearchPages? GetSearchCache();

        /// <summary>
        /// Sets the search cache for the current <see cref="HttpContext"/> HttpContext session.
        /// </summary>
        /// <param name="poemMetadata">The search results to cache. 
        /// Those searched results are chunked into pages, then saved.
        /// </param>
        /// <returns>The number of pages cached.</returns>
        int SetSearchCache(List<PersistedPoemMetadata> poemMetadata);
    }
}