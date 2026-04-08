using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Features.Poems
{
    public class PersistedPoemVersion
    {
        public required int PoemMetadataId { get; init; }
        public required int LanguageId { get; init; }
        public required bool IsOriginal { get; init; }
        public required string VersionText { get; init; }
    }
}