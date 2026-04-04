namespace PoetryGlobal.Domains.Poems.Models
{
    public class PoemMetadata
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public required string Author { get; init; }
    }
}