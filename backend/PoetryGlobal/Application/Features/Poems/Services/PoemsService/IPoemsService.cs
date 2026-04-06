

using PoetryGlobal.Features.Poems;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public interface IPoemsService
    {
        Task<List<PoemMetadataWithId>> SearchPoemsAsync(string titleQuery, string authorQuery);
        Task<PoemVersionWithId> GetPoemAsync(int poemId, int languageId);
    }
}