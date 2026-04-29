
namespace PoetryGlobal.Infrastructure
{
    /// <summary>
    /// Repository for languages.
    /// </summary>
    public interface ILanguageRepository
    {
        /// <summary>
        /// Retrieves all languages from the database.
        /// </summary>
        /// <returns>A list of models representing each language.</returns>
        Task<List<PersistedLanguage>> GetAllLanguagesAsync();
    }
}