namespace PoetryGlobal.Features.Poems
{
    public interface ITranslationService
    {
        Task<string> TranslatePoemAsync(string poemLinesMerged, string sourceLanguage, string targetLanguage);
    }
}