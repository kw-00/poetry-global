

namespace PoetryGlobal.Domains.Poems.Models
{ 
    public class PoemVersion : PoemMetadata
    {
        public required int LanguageId { get; init; }
        public required bool IsOriginal { get; init; }
        public required List<string> Lines { get; init; }

    }
}