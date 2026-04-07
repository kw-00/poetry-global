using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Features.Poems
{
    public class PoemMetadata : IPotentiallyPersistable
    {
        public int? Id { get; init; }
        public required string Title { get; init; }
        public required string Author { get; init; }

        public bool CanBePersisted() => Id is null;
    }
}