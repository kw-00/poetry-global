namespace PoetryGlobal.Infrastructure
{
    public interface ITranslationService
    {
        /// <summary>
        /// Translates a poem from one language to another.
        /// </summary>
        Task<string> TranslatePoemAsync(
            string sourceText, string sourceLanguage, string targetLanguage
        );
    }
}