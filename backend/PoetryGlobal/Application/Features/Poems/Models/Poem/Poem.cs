using System.Diagnostics.CodeAnalysis;
using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Features.Poems
{
    public class Poem : IPotentiallyPersistable
    {
        public int? Id { get; init; }
        public required string Title { get; init; }
        public required string Author { get; init; }
        public int? LanguageId { get; init; }
        public bool? IsOriginal { get; init; }
        public required string VersionText { get; init; }
        
        
        [MemberNotNullWhen(true, nameof(IsOriginal), nameof(LanguageId))]
        public bool CanBePersisted() {
            return Id is null && IsOriginal is not null && LanguageId is not null;
        }
    }
}

