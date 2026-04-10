using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Features.Poems
{
    public class PersistedPoemVersion : IPersistedModel<PoemVersion>
    {
        public required int PoemMetadataId { get; init; }
        public required int LanguageId { get; init; }
        public required bool IsOriginal { get; init; }
        public required string VersionText { get; init; }

        public PoemVersion ToNormalModel()
        {
            return new PoemVersion
            {
                PoemMetadataId = PoemMetadataId,
                IsOriginal = IsOriginal,
                LanguageId = LanguageId,
                VersionText = VersionText
            };
        }
    }
}