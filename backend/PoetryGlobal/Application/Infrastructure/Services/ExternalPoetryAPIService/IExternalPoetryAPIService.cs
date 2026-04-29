
namespace PoetryGlobal.Infrastructure
{
    public interface IExternalPoetryAPIService
    {
        /// <summary>
        /// Retrieves poem metadata from an external API.
        /// </summary>
        /// <param name="query">Object representation of the search query.</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<List<PoemMetadata>> GetPoemMetadataAsync(
            SearchQueryDTO query, int limit
        );
        /// <summary>
        /// Retrieves poems from an external API.
        /// </summary>
        /// <param name="titleQuery">Poem title or part of it.</param>
        /// <param name="authorQuery">Author or part of their name.</param>
        /// <param name="limit">Maximum number of poems to retrieve.</param>
        /// <returns></returns>
        Task<List<(PoemMetadata PoemMetadata, PoemVersion PoemVersion)>> GetPoemsAsync(
            SearchQueryDTO query, int limit
        );


    }
}