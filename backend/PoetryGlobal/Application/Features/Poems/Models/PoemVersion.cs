

namespace PoetryGlobal.Features.Poems
{
    public class PoemVersion
    {
        public required string Title { get; init; }
        public required string Author { get; init; }
        public required int LanguageId { get; init; }
        public required bool IsOriginal { get; init; }
        public required string Content { get; init; }

    }
}