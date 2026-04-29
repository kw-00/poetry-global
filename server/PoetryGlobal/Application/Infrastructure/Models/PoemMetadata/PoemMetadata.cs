
namespace PoetryGlobal.Infrastructure
{
    public class PoemMetadata : IModel
    {
        public int? Id { get; init; }
        public required string Title { get; init; }
        public required string Author { get; init; }

        public bool CanBePersisted() => Id is null;
    }
}