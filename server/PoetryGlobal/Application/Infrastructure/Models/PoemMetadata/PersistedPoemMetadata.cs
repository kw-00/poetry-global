

namespace PoetryGlobal.Infrastructure
{
    public class PersistedPoemMetadata : IPersistedModel<PoemMetadata>
    {
        public required int Id { get; init; }
        public required string Title { get; init; }
        public required string Author { get; init; }

        public PoemMetadata ToNormalModel()
        {
            return new PoemMetadata
            {
                Id = Id,
                Title = Title,
                Author = Author
            };
        }
    }
}