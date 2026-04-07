

using System.Text;
using Npgsql;

namespace PoetryGlobal.Features.Poems
{
    public class PoemRepository(NpgsqlDataSource dataSource) : IPoemRepository
    {
        private readonly NpgsqlDataSource _dataSource = dataSource;

        public async Task<List<PersistedPoemMetadata>> SearchPoemsAsync(string titleQuery, string authorQuery)
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
            var poemsMetadata = new List<PersistedPoemMetadata>();
            while (await reader.ReadAsync())
            {
                var poem = new PersistedPoemMetadata
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2)
                };
                poemsMetadata.Add(poem);
            }
            return poemsMetadata;
        }

        public async Task<PersistedPoem?> GetPoemVersionAsync(int poemId, int languageId)
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
                var poemVersion = new PersistedPoem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    LanguageId = reader.GetInt32(3),
                    IsOriginal = reader.GetBoolean(4),
                    VersionText = reader.GetString(5)
                };
                return poemVersion;
            }
            return null;
        }


        public async Task<List<PersistedPoemMetadata>> SavePoemOriginalsAsync(List<Poem> originalPoemVersions)
        {
            var query = _dataSource.CreateCommand();
            var queryStringBuilder = new StringBuilder("INSERT INTO poem_versions (title, author, language_id, is_original, content) VALUES ");
            var sqlTupleStrings = new List<string>();
            for (int i = 0; i < originalPoemVersions.Count; i++)
            {
                var originalPoemVersion = originalPoemVersions[i];
                if (!originalPoemVersion.CanBePersisted())
                {
                    throw new InvalidOperationException(
                        $"Poem version at index {i} cannot be persisted. Ensure it has no Id and all required fields are set."
                    );
                }
                sqlTupleStrings.Add($"@title{i}, @author{i}, @languageId{i}, @isOriginal{i}, @content{i}");
                query.Parameters.AddWithValue($"title{i}", originalPoemVersion.Title);
                query.Parameters.AddWithValue($"author{i}", originalPoemVersion.Author);
                query.Parameters.AddWithValue($"languageId{i}", originalPoemVersion.LanguageId);
                query.Parameters.AddWithValue($"isOriginal{i}", originalPoemVersion.IsOriginal);
                query.Parameters.AddWithValue($"content{i}", originalPoemVersion.VersionText);
            }
            queryStringBuilder.Append(string.Join(", ", sqlTupleStrings)).Append(" RETURNING id, title, author;");
            query.CommandText = queryStringBuilder.ToString();

            await using var reader = await query.ExecuteReaderAsync();
            var poemsMetadata = new List<PersistedPoemMetadata>();
            while (await reader.ReadAsync())
            {
                var poem = new PersistedPoemMetadata
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2)
                };
                poemsMetadata.Add(poem);
            }
            return poemsMetadata;

        }

        public async Task SavePoemVersionAsync(Poem poemVersion)
        {
            var query = _dataSource.CreateCommand(
                @"
                INSERT INTO poem_versions (title, author, language_id, is_original, content) 
                VALUES (@title, @author, @languageId, @isOriginal, @content)
                "
            );
            if (!poemVersion.CanBePersisted())
            {
                throw new InvalidOperationException("Poem version cannot be persisted. Ensure it has no Id and all required fields are set.");
            }
            query.Parameters.AddWithValue("title", poemVersion.Title);
            query.Parameters.AddWithValue("author", poemVersion.Author);
            query.Parameters.AddWithValue("languageId", poemVersion.LanguageId);
            query.Parameters.AddWithValue("isOriginal", poemVersion.IsOriginal);
            query.Parameters.AddWithValue("content", poemVersion.VersionText);

            await query.ExecuteNonQueryAsync();
        }

    }
}