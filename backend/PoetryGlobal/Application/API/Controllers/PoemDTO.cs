

using PoetryGlobal.Infrastructure;

namespace PoetryGlobal.API
{
    public class PoemDTO(PersistedPoemMetadata poemMetadata, PersistedPoemVersion poemVersion)
    {
        public int Id { get; init; } = poemMetadata.Id;
        public string Title { get; init; } = poemMetadata.Title;
        public string Author { get; init; } = poemMetadata.Author;
        public int LanguageId { get; init; } = poemVersion.LanguageId;
        public bool IsOriginal { get; init; } = poemVersion.IsOriginal;
        public string VersionText { get; init; } = poemVersion.VersionText;
    }
}