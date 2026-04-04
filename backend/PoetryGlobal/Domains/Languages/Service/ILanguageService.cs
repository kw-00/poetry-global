


using PoetryGlobal.Domains.Languages.Models;

namespace PoetryGlobal.Domains.Languages.Service
{
    public interface ILanguageService
    {
        Task<List<Language>> GetAllLanguages();
    }
}