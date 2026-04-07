namespace PoetryGlobal.Features.Poems {
    public interface ILanguageCache
    {
        Task<string?> GetLanguageCodeAsync(int languageId);
    }
}