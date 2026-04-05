


using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public class GetPoemResponse
    {
        public required PoemVersionWithId Poem { get; init; }
    }
}