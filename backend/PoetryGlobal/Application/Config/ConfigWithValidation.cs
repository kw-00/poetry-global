using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Config
{
    /// <summary>
    /// IConfigWithValidation implementation that uses injected IConfiguration to get configuration variables.
    /// </summary>
    public class ConfigWithParsing : IConfigWithValidation
    {
        private readonly IConfiguration? _configuration;
        public ConfigWithParsing(IConfiguration configuration) => _configuration = configuration;
        public ConfigWithParsing() => _configuration = null;
        public T GetFromConfigOrThrow<T>(string key, IFormatProvider? formatProvider = null) where T : IParsable<T>
        {
            if (_configuration is null) throw new NullReferenceException(
                "IConfiguration was not injected, but is required for this method."
            );
            var value = _configuration[key] ?? throw new AppSettingsKeyNotFoundException(key);
            return T.Parse(value, formatProvider);
        }

        public T GetFromEnvOrThrow<T>(string key, IFormatProvider? formatProvider = null) where T : IParsable<T>
        {
            var value = Environment.GetEnvironmentVariable(key) ?? throw new EnvironmentVariableNotSetException(key);
            return T.Parse(value, formatProvider);
        }
    }
}