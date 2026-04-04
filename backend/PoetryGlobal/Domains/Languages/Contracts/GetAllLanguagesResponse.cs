
using PoetryGlobal.Domains.Languages.Models;

namespace PoetryGlobal.Domains.Languages.Contracts
{
    public class GetAllLanguagesResponse
    {
        public required List<Language> Languages { get; init; }
    }
}