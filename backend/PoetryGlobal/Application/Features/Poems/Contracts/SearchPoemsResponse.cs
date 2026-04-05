



using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public class SearchPoemsResponse
    {
        public required List<PoemMetadataWithId> PoemsMetadata { get; init; }
    }
}