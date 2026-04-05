namespace PoetryGlobal.Application.Exceptions
{
    public class AppSettingsKeyNotFound(string keys) : Exception($"AppSettings key '{keys}' not found.");
}