namespace PoetryGlobal.Application.Exceptions
{
    public class AppSettingsKeyNotFoundException(string keys) : Exception($"AppSettings key '{keys}' not found.");
}