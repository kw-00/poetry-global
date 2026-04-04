


using PoetryGlobal.Domains.Languages.Contracts;
using PoetryGlobal.Domains.Languages.Models;

namespace PoetryGlobal.Domains.Languages.Service
{
    public interface ILanguageService
    {
        Task<GetAllLanguagesResponse> GetAllLanguagesAsync();
    }
}