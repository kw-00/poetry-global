

using PoetryGlobal.Features.Poems;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public class PoetryDbService : IPoetryDbService
    {
        public Task<List<PoemVersion>> GetPoemsAsync(string titleQuery, string authorQuery)
        {
            throw new NotImplementedException();
        }
    }
}