

using PoetryGlobal.Domains.Poems.Contracts;
using PoetryGlobal.Domains.Poems.Models;

namespace PoetryGlobal.Domains.Poems.Services.PoetryDbService
{
    public interface IPoetryDbService
    {
        Task<List<PoemVersion>> GetPoemsAsync(string titleQuery, string authorQuery);
    }
}