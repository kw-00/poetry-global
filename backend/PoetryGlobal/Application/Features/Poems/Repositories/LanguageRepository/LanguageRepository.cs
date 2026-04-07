using Npgsql;

namespace PoetryGlobal.Features.Poems
{
    public class LanguageRepository(NpgsqlDataSource dataSource) : ILanguageRepository
    {
        private readonly NpgsqlDataSource _dataSource = dataSource;

        public async Task<List<PersistedLanguage>> GetAllLanguagesAsync()
        {
            await using var query = _dataSource.CreateCommand(@"
                SELECT id, code FROM app.languages;
            ");
            await using var reader = await query.ExecuteReaderAsync();
            var languages = new List<PersistedLanguage>();
            while (await reader.ReadAsync())
            {
                var language = new PersistedLanguage
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1)
                };
                languages.Add(language);
            }
            return languages;
        }

    }
}