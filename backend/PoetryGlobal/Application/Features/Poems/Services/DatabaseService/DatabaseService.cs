

using Npgsql;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public class DatabaseService(NpgsqlDataSource dataSource) : IDatabaseService
    {
        private readonly NpgsqlDataSource _dataSource = dataSource;

        public async Task<List<PoemMetadataWithId>> SearchForPoemsAsync(string titleQuery, string authorQuery)
        {
            var query = _dataSource.CreateCommand(
                @"
                SELECT id, title, author 
                FROM poem_metadata 
                WHERE title <% @titleQuery AND author <% @authorQuery
                "
            );
            query.Parameters.AddWithValue("titleQuery", titleQuery);
            query.Parameters.AddWithValue("authorQuery", authorQuery);

            await using var reader = await query.ExecuteReaderAsync();
            var poemsMetadata = new List<PoemMetadataWithId>();
            while (await reader.ReadAsync())
            {
                var poem = new PoemMetadataWithId
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2)
                };
                poemsMetadata.Add(poem);
            }
            return poemsMetadata;
        }

        public Task<PoemVersionWithId> GetPoemVersionAsync(int poemId, int languageId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PoemMetadataWithId>> SavePoemOriginalsAsync(List<PoemVersion> originalPoemVersions)
        {
            throw new NotImplementedException();
        }

        public Task SavePoemVersionAsync(PoemVersion poemVersion)
        {
            throw new NotImplementedException();
        }

    }
}