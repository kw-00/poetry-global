using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Features.Poems
{
    public class PersistedLanguage
    {
        public required int Id { get; init; }
        public required string Code { get; init; }
    }
}