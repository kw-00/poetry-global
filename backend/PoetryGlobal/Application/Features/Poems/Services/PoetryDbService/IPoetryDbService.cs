
namespace PoetryGlobal.Features.Poems
{
    public interface IPoetryDbService
    {
        Task<List<(PoemMetadata PoemMetadata, PoemVersion PoemVersion)>> GetPoemsAsync(
            string titleQuery, string authorQuery, int limit
        );
    }
}