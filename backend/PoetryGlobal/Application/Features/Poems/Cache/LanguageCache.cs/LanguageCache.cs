using Npgsql;
using PoetryGlobal.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public class LanguageCache(NpgsqlDataSource dataSource) : ILanguageCache
    {
        private static bool _isInitialized = false;
        private readonly NpgsqlDataSource _dataSource = dataSource;
        private readonly ISimpleCache<int, string> _idToCodeCache = new SimpleCache<int, string>(60 * 10);
        private readonly ISimpleCache<string, int> _codeToIdCache = new SimpleCache<string, int>(60 * 10);

        public async Task<string?> GetLanguageCodeAsync(int languageId)
        {
            await InitializeCacheIfNeededAsync();
            return _idToCodeCache.Get(languageId);
        }

        public async Task<int?> GetLanguageIdAsync(string languageCode)
        {
            await InitializeCacheIfNeededAsync();
            return _codeToIdCache.Get(languageCode);
        }

        private async Task InitializeCacheIfNeededAsync()
        {
            if (!_isInitialized)
            {
                var query = _dataSource.CreateCommand(@"
                    SELECT id, code 
                    FROM languages;
                ");

                await using var reader = await query.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var code = reader.GetString(1);
                    _idToCodeCache.Set(id, code, null);
                    _codeToIdCache.Set(code, id, null);
                }
                _isInitialized = true;
            }
        }
    }
}