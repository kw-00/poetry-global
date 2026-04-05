

using PoetryGlobal.Features.Poems;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public interface IPoetryDbService
    {
        Task<List<PoemVersion>> GetPoemsAsync(string titleQuery, string authorQuery);
    }
}