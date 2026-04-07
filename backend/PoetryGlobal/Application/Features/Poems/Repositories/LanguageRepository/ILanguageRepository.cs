
namespace PoetryGlobal.Features.Poems
{
    public interface ILanguageRepository
    {
        Task<List<PersistedLanguage>> GetAllLanguagesAsync();
    }
}