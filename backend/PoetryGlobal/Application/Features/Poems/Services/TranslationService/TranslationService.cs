using System.Text.Json;
using PoetryGlobal.ConfigWithParsing;
using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Features.Poems
{
    public class TranslationService(HttpClient httpClient, IConfigWithValidation configuration, JsonSerializerOptions jsonSerializerOptions) : ITranslationService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _myMemoryBaseUrl 
            = configuration.GetFromConfigOrThrow<string>("ExternalApis:MyMemory:BaseUrl");

        public async Task<string> TranslatePoemAsync(
            string poemLinesMerged, string sourceLanguage, string targetLanguage
        )
        {
            var sourceLines = poemLinesMerged.Split("\n");
            var targetLines = new List<string>();

            foreach (var line in sourceLines) 
            {
                var escapedSourceLanguage = Uri.EscapeDataString(sourceLanguage);
                var escapedTargetLanguage = Uri.EscapeDataString(targetLanguage);
                var url = $"{_myMemoryBaseUrl}/get?q={line}&langpair={escapedSourceLanguage}|{escapedTargetLanguage}";
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Remove("Accept");
                requestMessage.Headers.Add("Accept", "application/json");
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                var responseObject = await response.Content.ReadFromJsonAsync<MyMemoryResponse>(_jsonSerializerOptions) 
                    ?? throw new NullReferenceException("Deserialized response is null.");

                targetLines.Add(responseObject.ResponseData.TranslatedText);
            }
            return string.Join("\n", targetLines);
        }
    }

    internal class MyMemoryResponse
    {
        public required Data ResponseData { get; set; } 

        internal class Data
        {
            public required string TranslatedText { get; set; }
        }
    }
}