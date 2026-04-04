using System.Runtime.CompilerServices;
using PoetryGlobal.Domains.Poems.Contracts;
using PoetryGlobal.Domains.Poems.Models;

namespace PoetryGlobal.Domains.Poems.Services.DatabaseService
{
    public interface IDatabaseService
    {
        Task<List<PoemMetadataWithId>> SearchForPoemsAsync(string titleQuery, string authorQuery);
        Task<PoemVersionWithId> GetPoemVersionAsync(int poemId, int languageId);

        Task<List<PoemMetadataWithId>> SavePoemOriginalsAsync(List<PoemVersion> originalPoemVersions);

        Task SavePoemVersionAsync(PoemVersion poemVersion);
    }
}