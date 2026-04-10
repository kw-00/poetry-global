namespace PoetryGlobal.SharedDIDependencies
{
    /// <summary>
    /// Provides methods for comfortable parsing of configuration and environment variables.
    /// </summary>
    public interface IConfigWithValidation
    {
        /// <summary>
        /// Retrieves a value from configuration. Throws an exception 
        /// if the value cannot be found or parsed.
        /// and parses it using see paremeter <typeparamref name="T.Parse()"/>.
        /// </summary>
        /// <typeparam name="T">The type to parse the value to.</typeparam>
        /// <param name="key">Configuration key.</param>
        /// <param name="formatProvider"><see cref="IFormatProvider"/> for parsing. Can be null.</param>
        /// <returns>The parsed variable value.</returns>
        T GetFromConfigOrThrow<T>(string key, IFormatProvider? formatProvider = null) where T : IParsable<T>;

        /// <summary>
        /// Retrieves a value from environment variable. Throws an exception 
        /// if the value cannot be found or parsed.
        /// </summary>
        /// <typeparam name="T">The type to parse the value to.</typeparam>
        /// <param name="key">Environment variable name.</param>
        /// <param name="formatProvider"><see cref="IFormatProvider"/> for parsing. Can be null.</param>
        /// <returns>The parsed variable value.</returns>
        T GetFromEnvOrThrow<T>(string key, IFormatProvider? formatProvider = null) where T : IParsable<T>;
    }
}