
namespace PoetryGlobal.Features.Poems
{
    public interface IPoetryDbService
    {
        Task<List<Poem>> GetPoemsAsync(string titleQuery, string authorQuery);
    }
}