

using PoetryGlobal.Domains.Poems.Contracts;
using PoetryGlobal.Domains.Poems.Models;

namespace PoetryGlobal.Domains.Poems.Services
{
    public interface IPoemsService
    {
        Task<List<PoemMetadataWithId>> DatabaseSearchAsync(string titleQuery, string authorQuery);
        Task<List<PoemMetadataWithId>> PoetryDbSearchAsync(string titleQuery, string authorQuery);
        Task<PoemVersionWithId> GetPoemAsync(int poemId, int languageId);
    }
}