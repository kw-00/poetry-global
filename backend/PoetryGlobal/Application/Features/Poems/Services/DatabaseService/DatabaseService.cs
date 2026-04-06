

using System.Text;
using Npgsql;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public class DatabaseService(NpgsqlDataSource dataSource) : IDatabaseService
    {
        private readonly NpgsqlDataSource _dataSource = dataSource;

        public async Task<List<PoemMetadataWithId>> SearchPoemsAsync(string titleQuery, string authorQuery)
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

        public async Task<PoemVersionWithId?> GetPoemVersionAsync(int poemId, int languageId)
        {
            var query = _dataSource.CreateCommand(
                @"
                SELECT id, title, author, language_id, is_original, content
                FROM poem_versions 
                WHERE id = @poemId AND language_id = @languageId
                "
            );
            query.Parameters.AddWithValue("poemId", poemId);
            query.Parameters.AddWithValue("languageId", languageId);

            await using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var poemVersion = new PoemVersionWithId
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    LanguageId = reader.GetInt32(3),
                    IsOriginal = reader.GetBoolean(4),
                    Content = reader.GetString(5)
                };
                return poemVersion;
            }
            return null;
        }


        public async Task<List<PoemMetadataWithId>> SavePoemOriginalsAsync(List<PoemVersion> originalPoemVersions)
        {
            var query = _dataSource.CreateCommand();
            var queryStringBuilder = new StringBuilder("INSERT INTO poem_versions (title, author, language_id, is_original, content) VALUES ");
            var sqlTupleStrings = new List<string>();
            for (int i = 0; i < originalPoemVersions.Count; i++)
            {
                sqlTupleStrings.Add($"@title{i}, @author{i}, @languageId{i}, @isOriginal{i}, @content{i}");
                query.Parameters.AddWithValue($"title{i}", originalPoemVersions[i].Title);
                query.Parameters.AddWithValue($"author{i}", originalPoemVersions[i].Author);
                query.Parameters.AddWithValue($"languageId{i}", originalPoemVersions[i].LanguageId);
                query.Parameters.AddWithValue($"isOriginal{i}", originalPoemVersions[i].IsOriginal);
                query.Parameters.AddWithValue($"content{i}", originalPoemVersions[i].Content);
            }
            queryStringBuilder.Append(string.Join(", ", sqlTupleStrings)).Append(" RETURNING id, title, author;");
            query.CommandText = queryStringBuilder.ToString();

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

        public async Task SavePoemVersionAsync(PoemVersion poemVersion)
        {
            var query = _dataSource.CreateCommand(
                @"
                INSERT INTO poem_versions (title, author, language_id, is_original, content) 
                VALUES (@title, @author, @languageId, @isOriginal, @content)
                "
            );
            query.Parameters.AddWithValue("title", poemVersion.Title);
            query.Parameters.AddWithValue("author", poemVersion.Author);
            query.Parameters.AddWithValue("languageId", poemVersion.LanguageId);
            query.Parameters.AddWithValue("isOriginal", poemVersion.IsOriginal);
            query.Parameters.AddWithValue("content", poemVersion.Content);

            await query.ExecuteNonQueryAsync();
        }

    }
}