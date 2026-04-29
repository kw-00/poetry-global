using PoetryGlobal.Interfaces;

namespace PoetryGlobal.Infrastructure
{
    public class PersistedLanguage : IPersistedModel<Language>
    {
        public required int Id { get; init; }
        public required string Code { get; init; }

        public Language ToNormalModel()
        {
            return new Language
            {
                Id = Id,
                Code = Code
            };
        }
    }
}