using System.Text;
using Npgsql;
using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Features.Poems
{
    public class PoemMetadataRepository(NpgsqlDataSource dataSource, ILogger<PoemMetadataRepository> logger) : IPoemMetadataRepository
    {
        private readonly ILogger<PoemMetadataRepository> _logger = logger;
        private readonly NpgsqlDataSource _dataSource = dataSource;
        public async Task<List<PersistedPoemMetadata>> SearchAsync(
            string titleQuery, string authorQuery, int limit
        )
        {
            var query = _dataSource.CreateCommand(@"
                SELECT id, title, author 
                FROM poem_metadata 
                WHERE (title <% @titleQuery OR title % @titleQuery) 
                    AND (author <% @authorQuery OR author % @authorQuery) 
                ORDER BY 
                    word_similarity(@titleQuery, title),
                    similarity(@titleQuery, title),
                    word_similarity(@authorQuery, author),
                    similarity(@authorQuery, author)
                DESC
                LIMIT @limit;
            ");
            query.Parameters.AddWithValue("titleQuery", titleQuery);
            query.Parameters.AddWithValue("authorQuery", authorQuery);
            query.Parameters.AddWithValue("limit", limit);

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

        public async Task<PersistedPoemMetadata?> GetAsync(int id)
        {
            var query = _dataSource.CreateCommand(@"
                SELECT id, title, author 
                FROM poem_metadata 
                WHERE id = @id
                LIMIT 1;
            ");
            query.Parameters.AddWithValue("id", id);
            
            await using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PersistedPoemMetadata
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2)
                };
            }
            return null;
        }

        public async Task<PersistedPoemMetadata> UpsertAsync(PoemMetadata poemMetadata)
        {
            if (!poemMetadata.CanBePersisted()) 
                throw new ModelNotPersistableException("Poem metadata cannot be persisted.");

            var query = _dataSource.CreateCommand(@"
                INSERT INTO poem_metadata (title, author) 
                VALUES (@title, @author) 
                ON CONFLICT (title, author) 
                    DO UPDATE SET
                        title = EXCLUDED.title, 
                        author = EXCLUDED.author
                RETURNING id, title, author;
            ");
            query.Parameters.AddWithValue("title", poemMetadata.Title);
            query.Parameters.AddWithValue("author", poemMetadata.Author);
            
            await using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PersistedPoemMetadata
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2)
                };
            }
            throw new ResultsetUnexpectedlyMissingException("Poem metadata unexpectedly missing from query reader.");
        }

        public async Task<List<PersistedPoemMetadata>> UpsertAllAsync(List<PoemMetadata> poemsMetadata)
        {
            if (poemsMetadata.Count == 0) return [];
            var query = _dataSource.CreateCommand();
            var queryStringBuilder = new StringBuilder("INSERT INTO poem_metadata (title, author) VALUES");
            var insertionTupleStrings = new List<string>();
            for (int i = 0; i < poemsMetadata.Count; i++)
            {
                var poemMetadata = poemsMetadata[i];
                if (!poemMetadata.CanBePersisted()) throw new ModelNotPersistableException("Poem metadata cannot be persisted.");
                insertionTupleStrings.Add($"(@title{i}, @author{i})");
                query.Parameters.AddWithValue($"title{i}", poemMetadata.Title);
                query.Parameters.AddWithValue($"author{i}", poemMetadata.Author);
            }
            queryStringBuilder.Append(string.Join("\n,", insertionTupleStrings))
            .Append("\nON CONFLICT (title, author) DO UPDATE SET title = EXCLUDED.title, author = EXCLUDED.author")
            .Append("\nRETURNING id, title, author;");

            query.CommandText = queryStringBuilder.ToString();
            await using var reader = await query.ExecuteReaderAsync();
            var resultPoemMetadata = new List<PersistedPoemMetadata>();
            while (await reader.ReadAsync())
            {
                resultPoemMetadata.Add(new PersistedPoemMetadata
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2)
                });
            }
            return resultPoemMetadata;
        }
    }
}