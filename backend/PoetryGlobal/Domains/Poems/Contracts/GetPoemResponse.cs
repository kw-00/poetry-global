


using PoetryGlobal.Domains.Poems.Models;

namespace PoetryGlobal.Domains.Poems.Contracts
{
    public class GetPoemResponse
    {
        public required PoemVersion Poem { get; init; }
    }
}