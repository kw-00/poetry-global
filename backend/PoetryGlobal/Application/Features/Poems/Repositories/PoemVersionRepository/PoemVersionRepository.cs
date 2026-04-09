

using System.Text;
using Npgsql;
using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Features.Poems
{
    public class PoemVersionRepository(NpgsqlDataSource dataSource) : IPoemVersionRepository
    {
        private readonly NpgsqlDataSource _dataSource = dataSource;

        public async Task<PersistedPoemVersion?> GetAsync(int poemMetadataId, int languageId)
        {
            var query = _dataSource.CreateCommand(@"
                SELECT poem_metadata_id, is_original, language_id, version_text
                FROM poem_versions 
                WHERE poem_metadata_id = @poemMetadataId AND language_id = @languageId
                LIMIT 1;
            ");
            query.Parameters.AddWithValue("poemMetadataId", poemMetadataId);
            query.Parameters.AddWithValue("languageId", languageId);
            
            await using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PersistedPoemVersion
                {
                    PoemMetadataId = reader.GetInt32(0),
                    IsOriginal = reader.GetBoolean(1),
                    LanguageId = reader.GetInt32(2),
                    VersionText = reader.GetString(3)
                };
            }
            return null;
        }

        public async Task<PersistedPoemVersion?> GetOriginalAsync(int poemMetadataId)
        {
            var query = _dataSource.CreateCommand(@"
                SELECT poem_metadata_id, is_original, language_id, version_text
                FROM poem_versions 
                WHERE poem_metadata_id = @poemMetadataId AND is_original = true
                LIMIT 1;
            ");
            query.Parameters.AddWithValue("poemMetadataId", poemMetadataId);
            
            await using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PersistedPoemVersion
                {
                    PoemMetadataId = reader.GetInt32(0),
                    IsOriginal = reader.GetBoolean(1),
                    LanguageId = reader.GetInt32(2),
                    VersionText = reader.GetString(3)
                };
            }
            return null;
        }

        public async Task<List<PersistedPoemVersion>> UpsertAllAsync(List<PoemVersion> poemVersions)
        {
            if (poemVersions.Count == 0) return [];
            var query = _dataSource.CreateCommand();
            var queryStringBuilder = new StringBuilder(
                @"INSERT INTO poem_versions (poem_metadata_id, is_original, language_id, version_text) VALUES "
            );
            var insertionTupleStrings = new List<string>();
            for (int i = 0; i < poemVersions.Count; i++)
            {
                var poemVersion = poemVersions[i];
                if (!poemVersion.CanBePersisted()) throw new ModelNotPersistableException("Poem version cannot be persisted.");
                insertionTupleStrings.Add($"(@poemMetadataId{i}, @isOriginal{i}, @languageId{i}, @versionText{i})");
                query.Parameters.AddWithValue($"poemMetadataId{i}", poemVersion.PoemMetadataId);
                query.Parameters.AddWithValue($"isOriginal{i}", poemVersion.IsOriginal);
                query.Parameters.AddWithValue($"languageId{i}", poemVersion.LanguageId);
                query.Parameters.AddWithValue($"versionText{i}", poemVersion.VersionText);
            }
            queryStringBuilder.Append(string.Join("\n,", insertionTupleStrings))
            .Append(
                "\nON CONFLICT (poem_metadata_id, language_id) DO UPDATE SET version_text = EXCLUDED.version_text"
            )
            .Append(
                "\nRETURNING poem_metadata_id, is_original, language_id, version_text;"
            );
            query.CommandText = queryStringBuilder.ToString();
            
            await using var reader = await query.ExecuteReaderAsync();
            var resultPoemVersions = new List<PersistedPoemVersion>();
            while (await reader.ReadAsync())
            {
                resultPoemVersions.Add(new PersistedPoemVersion
                {
                    PoemMetadataId = reader.GetInt32(0),
                    IsOriginal = reader.GetBoolean(1),
                    LanguageId = reader.GetInt32(2),
                    VersionText = reader.GetString(3)
                });
            }
            return resultPoemVersions;
        }

        public async Task<PersistedPoemVersion> UpsertAsync(PoemVersion poemVersion)
        {
            if (!poemVersion.CanBePersisted()) 
                throw new ModelNotPersistableException("Poem version cannot be persisted.");
            
            var query = _dataSource.CreateCommand(@"
                INSERT INTO poem_versions (poem_metadata_id, is_original, language_id, version_text) 
                VALUES (@poemMetadataId, @isOriginal, @languageId, @versionText)
                ON CONFLICT (poem_metadata_id, language_id) 
                    DO UPDATE SET version_text = EXCLUDED.version_text
                RETURNING poem_metadata_id, is_original, language_id, version_text;
            ");

            query.Parameters.AddWithValue("poemMetadataId", poemVersion.PoemMetadataId);
            query.Parameters.AddWithValue("isOriginal", poemVersion.IsOriginal);
            query.Parameters.AddWithValue("languageId", poemVersion.LanguageId);
            query.Parameters.AddWithValue("versionText", poemVersion.VersionText);
            await using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PersistedPoemVersion
                {
                    PoemMetadataId = reader.GetInt32(0),
                    IsOriginal = reader.GetBoolean(1),
                    LanguageId = reader.GetInt32(2),
                    VersionText = reader.GetString(3)
                };
            }
            throw new ResultSetUnexpectedlyMissingException(
                "Poem version unexpectedly missing from query reader."
            );
        }
    }
}