

using PoetryGlobal.Domains.Poems.Contracts;
using PoetryGlobal.Domains.Poems.Models;

namespace PoetryGlobal.Domains.Poems.Services.PoetryDbService
{
    public class PoetryDbService : IPoetryDbService
    {
        public Task<List<PoemVersion>> GetPoemsAsync(string titleQuery, string authorQuery)
        {
            throw new NotImplementedException();
        }
    }
}