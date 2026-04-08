

using System.Text;
using System.Text.Json;
using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Features.Poems
{

    public class PoetryDbService(
        ILanguageCache languageCache, 
        HttpClient httpClient, 
        IConfiguration configuration, 
        JsonSerializerOptions jsonSerializerOptions

    ) : IPoetryDbService
    {
        private readonly ILanguageCache _languageCache = languageCache;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;

        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;


        private static readonly string _poetryDbBaseUrlKey = "ExternalApis:PoetryDb:BaseUrl";
        private readonly string _poetryDbBaseUrl = configuration[_poetryDbBaseUrlKey] 
            ?? throw new AppSettingsKeyNotFoundException(_poetryDbBaseUrlKey);


        public async Task<List<(PoemMetadata PoemMetadata, PoemVersion PoemVersion)>> GetPoemsAsync(
            string authorQuery, string titleQuery, int limit
        )
        {
            var escapedAuthorQuery = Uri.EscapeDataString(authorQuery);
            var escapedTitleQuery = Uri.EscapeDataString(titleQuery);
            var url = $"{_poetryDbBaseUrl}/author,title,poemcount" 
                + $"/{escapedAuthorQuery};{escapedTitleQuery};{limit}/author,title,lines";

            var message = new HttpRequestMessage(HttpMethod.Get, url);
            message.Headers.Remove("Accept");
            message.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(message);

            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync()
                ?? throw new NullReferenceException("Deserialized poems list is null.");
            
            try
            {
                var poems = JsonSerializer.Deserialize<List<PoetryDbDataResponse>>(bytes, _jsonSerializerOptions) 
                    ?? throw new NullReferenceException("Deserialized poems list is null.");

                var englishLanguageId = await _languageCache.GetLanguageIdAsync("en")
                    ?? throw new KeyNotFoundException(
                        "English not found among languages, even though poems from PoetryDb are in English."
                    );

                return [.. poems.Select(p =>(
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

    }

    internal class PoetryDbDataResponse
    {
        public required string Title { get; init; }
        public required string Author { get; init; }
        public required List<string> Lines { get; init; }
    }

    internal class PoetryDbErrorResponse
    {
        public required int Status { get; init; }
        public required string Reason { get; init; }
    }
}