


using System.Data.Common;
using Npgsql;
using PoetryGlobal.Domains.Languages.Contracts;
using PoetryGlobal.Domains.Languages.Models;

namespace PoetryGlobal.Domains.Languages.Service
{
    public class LanguageService(NpgsqlDataSource dataSource) : ILanguageService
    {
        private readonly NpgsqlDataSource _dataSource = dataSource;

        public async Task<GetAllLanguagesResponse> GetAllLanguagesAsync()
        {
            await using var query = _dataSource.CreateCommand(
                """
                SELECT id, code FROM app.languages;
                """
            );
            await using var reader = await query.ExecuteReaderAsync();
            var languages = new List<Language>();
            while (await reader.ReadAsync())
            {
                var language = new Language
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1)
                };
                languages.Add(language);
            }
            return new GetAllLanguagesResponse { Languages = languages };
        }

    }
}