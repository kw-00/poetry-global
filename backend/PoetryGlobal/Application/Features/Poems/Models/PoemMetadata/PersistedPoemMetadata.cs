using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Features.Poems
{
    public class PersistedPoemMetadata : IPersistedModel
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public required string Author { get; init; }
    }
}