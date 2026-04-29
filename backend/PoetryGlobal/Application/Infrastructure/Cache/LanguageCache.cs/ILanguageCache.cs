namespace PoetryGlobal.Infrastructure {
    /// <summary>
    /// Simple in-memory cache for languages available in app.
    /// Provides thread-safe access to a language's code based on its ID, and vice-versa.
    /// </summary>
    public interface ILanguageCache
    {
        /// <summary>
        /// Retrieves language code based on its ID.
        /// </summary>
        /// <returns>The language code, or null if not found.</returns>
        Task<string?> GetLanguageCodeAsync(int languageId);

        /// <summary>
        /// Retrieves language ID based on its code.
        /// </summary>
        /// <returns>The language ID, or null if not found.</returns>
        Task<int?> GetLanguageIdAsync(string languageCode);
    }
}