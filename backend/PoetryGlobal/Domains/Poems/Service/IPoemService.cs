

using PoetryGlobal.Domains.Poems.Contracts;

namespace PoetryGlobal.Domains.Poems.Service
{
    public interface IPoemService
    {
        Task<SearchPoemsResponse> SearchPoems(string title, string author);
        Task<GetPoemResponse> GetPoem(int poemId, int languageId);
    }
}