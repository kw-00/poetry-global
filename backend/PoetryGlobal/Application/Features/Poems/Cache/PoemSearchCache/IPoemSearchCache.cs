using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    /// <summary>
    /// In-memory cache for poem search results. Stores search results per session,
    /// so that every user can have one query result set cached at a time.
    /// 
    /// Provides thread-safe read/write access as well as simple eviction policy
    /// (check <see cref="ISimpleCache{K, V}"/>).
    /// </summary>
    public interface IPoemSearchCache : ISimpleCache<SearchQueryDTO, ISearchPages>;

    /// <summary>
    /// Represents a set of pages that correspond to a single search query.
    /// </summary>
    public interface ISearchPages
    {
        int PageCount { get; }

        /// <summary>
        /// Retrieves a cached search query page or null if not found or out of bounds.
        /// </summary>
        /// <param name="pageNumber">Zero-indexed page number.</param>
        /// <returns>Cached page or null if not found or out of bounds.</returns>
        PersistedPoemMetadata[]? GetPage(int pageNumber);
    }
}