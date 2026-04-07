using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Features.Poems
{
    public class TranslationService(HttpClient httpClient,IConfiguration configuration) : ITranslationService
    {
        private readonly HttpClient _httpClient = httpClient;
        private static readonly string _MY_MEMORY_BASE_URL_KEY = "ExternalApis:MyMemory:BaseUrl";
        private readonly string _myMemoryBaseUrl = configuration[_MY_MEMORY_BASE_URL_KEY] 
            ?? throw new AppSettingsKeyNotFoundException(_MY_MEMORY_BASE_URL_KEY);

        public async Task<string> TranslatePoemAsync(string poemLinesMerged, string sourceLanguage, string targetLanguage)
        {
            var sourceLines = poemLinesMerged.Split("\n");
            var targetLines = new List<string>();

            foreach (var line in sourceLines) 
            {
                var url = Uri.EscapeDataString(
                    $"{_myMemoryBaseUrl}get?q={poemLinesMerged}&langpair={sourceLanguage}|{targetLanguage}"
                );
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Remove("Accept");
                requestMessage.Headers.Add("Accept", "application/json");
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                var responseObject = await response.Content.ReadFromJsonAsync<MyMemoryResponse>() 
                    ?? throw new NullReferenceException("Deserialized response is null.");

                targetLines.Add(responseObject.ResponseData.translatedText);
            }
            return string.Join("\n", targetLines);
        }
    }

    internal class MyMemoryResponse
    {
        public required Data ResponseData { get; set; } 

        internal class Data
        {
            public string translatedText { get; set; } = string.Empty;
        }
    }
}