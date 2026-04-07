using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Features.Poems
{
    public class PersistedPoem
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public required string Author { get; init; }
        public required int LanguageId { get; init; }
        public required bool IsOriginal { get; init; }
        public required string VersionText { get; init; }
    }
}