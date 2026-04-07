namespace PoetryGlobal.Exceptions
{
    public class AppSettingsKeyNotFoundException(string keys) : Exception($"AppSettings key '{keys}' not found.");
}