

using PoetryGlobal.Features.Poems;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public interface IPoemsService
    {
        Task<List<PoemMetadataWithId>> DatabaseSearchAsync(string titleQuery, string authorQuery);
        Task<List<PoemMetadataWithId>> PoetryDbSearchAsync(string titleQuery, string authorQuery);
        Task<PoemVersionWithId> GetPoemAsync(int poemId, int languageId);
    }
}