



using PoetryGlobal.Domains.Poems.Models;

namespace PoetryGlobal.Domains.Poems.Contracts
{
    public class SearchPoemsResponse
    {
        public required List<PoemMetadataWithId> PoemsMetadata { get; init; }
    }
}