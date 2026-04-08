namespace PoetryGlobal.Features.Poems {
    public interface ILanguageCache
    {
        Task<string?> GetLanguageCodeAsync(int languageId);
        Task<int?> GetLanguageIdAsync(string languageCode);
    }
}