
namespace PoetryGlobal.Features.Poems
{
    public interface IPoetryDbService
    {
        /// <summary>
        /// Retrieves poems from an external API.
        /// </summary>
        /// <param name="titleQuery">Poem title or part of it.</param>
        /// <param name="authorQuery">Author or part of their name.</param>
        /// <param name="limit">Maximum number of poems to retrieve.</param>
        /// <returns></returns>
        Task<List<(PoemMetadata PoemMetadata, PoemVersion PoemVersion)>> GetPoemsAsync(
            string titleQuery, string authorQuery, int limit
        );
    }
}