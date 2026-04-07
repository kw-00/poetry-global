using Microsoft.AspNetCore.StaticAssets;
using Npgsql;
using PoetryGlobal.Shared.SimpleCache;

namespace PoetryGlobal.Features.Poems
{
    public class LanguageCache(NpgsqlDataSource dataSource) : ILanguageCache
    {
        private static bool _isInitialized = false;
        private readonly NpgsqlDataSource _dataSource = dataSource;
        private static readonly ISimpleCache<int, string> _cache = new SimpleCache<int, string>();

        public async Task<string?> GetLanguageCodeAsync(int languageId)
        {
            if (!_isInitialized)
            {
                var query = _dataSource.CreateCommand(@"
                    SELECT id, code 
                    FROM languages;
               ");
                using var reader = query.ExecuteReader();
                while (await reader.ReadAsync())
                {
                    _cache.Set(reader.GetInt32(0), reader.GetString(1), null);
                }
                _isInitialized = true;
            }
            return _cache.Get(languageId);
        }
    }
}