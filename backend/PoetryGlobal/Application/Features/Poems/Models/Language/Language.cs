
using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Features.Poems
{
    public class Language : IPotentiallyPersistable
    {
        public int? Id { get; init; }
        public required string Code { get; init; }


        public bool CanBePersisted() => Id is null;
    }
}