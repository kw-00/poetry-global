using System.Runtime.CompilerServices;
using PoetryGlobal.Features.Poems;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public interface IDatabaseService
    {
        Task<List<PoemMetadataWithId>> SearchPoemsAsync(string titleQuery, string authorQuery);
        Task<PoemVersionWithId?> GetPoemVersionAsync(int poemId, int languageId);

        Task<List<PoemMetadataWithId>> SavePoemOriginalsAsync(List<PoemVersion> originalPoemVersions);

        Task SavePoemVersionAsync(PoemVersion poemVersion);
    }
}