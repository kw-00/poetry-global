namespace PoetryGlobal.Features.Poems
{
    public class PersistedPoemMetadata
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public required string Author { get; init; }
    }
}