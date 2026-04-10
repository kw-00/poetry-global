

using System.Text;
using System.Text.Json;
using PoetryGlobal.SharedDIDependencies;
using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Features.Poems
{

    public class PoetryDbService(
        ILanguageCache languageCache, 
        HttpClient httpClient, 
        IConfigWithValidation configuration, 
        JsonSerializerOptions jsonSerializerOptions,

        ILogger<PoetryDbService> logger

    ) : IExternalPoetryAPIService
    {
        private readonly ILanguageCache _languageCache = languageCache;
        private readonly HttpClient _httpClient = httpClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;


        private readonly string _poetryDbBaseUrl
            = configuration.GetFromConfigOrThrow<string>("ExternalApis:PoetryDb:BaseUrl");

        
        public async Task<List<PoemMetadata>> GetPoemMetadataAsync(SearchQueryDTO query, int limit)
        {
            var data = await GetPoemsDataAsync<PoetryDbDataNoLines>(query, limit);
            return [..
                data.Select(data => new PoemMetadata 
                {
                    Title = data.Title,
                    Author = data.Author
                })
            ];
        }

        public async Task<List<(PoemMetadata PoemMetadata, PoemVersion PoemVersion)>> GetPoemsAsync(SearchQueryDTO query, int limit)
        {
            var data = await GetPoemsDataAsync<PoetryDbDataFull>(query, limit);
            var englishLanguageId = await _languageCache.GetLanguageIdAsync("en");
            return [.. data.Select(p =>(
                PoemMetadata: new PoemMetadata
                {
                    Author = p.Author,
                    Title = p.Title
                },
                PoemVersion: new PoemVersion 
                {
                    LanguageId = englishLanguageId,
                    IsOriginal = true,
                    VersionText = string.Join("\n", p.Lines),
                }
            ))];
        }


        private  async Task<List<T>> GetPoemsDataAsync<T>(
            SearchQueryDTO query, int limit
        ) where T : IPoetryDbData
        {
            var escapedTitleQuery = Uri.EscapeDataString(query.Title);
            var escapedAuthorQuery = Uri.EscapeDataString(query.Author);
            bool includeLines = typeof(T) == typeof(PoetryDbDataFull);
            var url = $"{_poetryDbBaseUrl}/title,author,poemcount" 
                + $"/{escapedTitleQuery};{escapedAuthorQuery};{limit}/title,author" 
                + (includeLines ? ",lines" : "") 
                + ".json";

            logger.LogCritical("Search query: {url}", url);

            var message = new HttpRequestMessage(HttpMethod.Get, url);
            message.Headers.Remove("Accept");
            message.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(message);

            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync()
                ?? throw new NullReferenceException("Deserialized poems list is null.");
            
            try
            {
                var data = JsonSerializer.Deserialize<List<T>>(bytes, _jsonSerializerOptions) 
                    ?? throw new NullReferenceException("Deserialized poems list is null.");

                var englishLanguageId = await _languageCache.GetLanguageIdAsync("en")
                    ?? throw new KeyNotFoundException(
                        "English not found among languages, even though poems from PoetryDb are in English."
                    );

                return data;
            }
            catch (JsonException)
            {
                var responseContent = JsonSerializer.Deserialize<PoetryDbErrorResponse>(
                    bytes, _jsonSerializerOptions
                );
                if (responseContent is not null && responseContent.Status == 404)
                {
                    return [];
                }
                throw;
            }
        }

        private interface IPoetryDbData;

        private class PoetryDbDataNoLines : IPoetryDbData
        {
            public required string Title { get; init; }
            public required string Author { get; init; }
        }

        private class PoetryDbDataFull : IPoetryDbData
        {
            public required string Title { get; init; }
            public required string Author { get; init; }
            public required List<string> Lines { get; init; }
        }

        private class PoetryDbErrorResponse
        {
            public required int Status { get; init; }
            public required string Reason { get; init; }
        }

    }
}