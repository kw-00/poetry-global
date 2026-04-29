using System.Diagnostics.CodeAnalysis;
using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Infrastructure
{
    public class PoemVersion : IModel
    {
        public int? PoemMetadataId { get; init; }
        public int? LanguageId { get; init; }
        public bool? IsOriginal { get; init; }
        public required string VersionText { get; init; }
        
        
        [MemberNotNullWhen(true, nameof(PoemMetadataId), nameof(IsOriginal), nameof(LanguageId))]
        public bool CanBePersisted() {
            return PoemMetadataId is not null && IsOriginal is not null && LanguageId is not null;
        }
    }
}

