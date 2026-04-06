

using PoetryGlobal.Application.Exceptions;

namespace PoetryGlobal.Features.Poems
{
    public class PoetryDbService(HttpClient httpClient, IConfiguration configuration) : IPoetryDbService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;


        private static readonly string _POETRY_DB_BASE_URL_KEY = "ExternalApis:PoetryDb:BaseUrl";
        private readonly string _poetryDbBaseUrl = configuration[_POETRY_DB_BASE_URL_KEY] 
            ?? throw new AppSettingsKeyNotFoundException(_POETRY_DB_BASE_URL_KEY);


        private static readonly string _BATCH_SIZE_KEY = "Features:Poems:PoetryDbBatchSize";
        private readonly string _batchSize = configuration[_BATCH_SIZE_KEY] 
            ?? throw new AppSettingsKeyNotFoundException(_BATCH_SIZE_KEY);

        public async Task<List<PoetryDbPoemLinesMerged>> GetPoemsAsync(string titleQuery, string authorQuery)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"{_poetryDbBaseUrl}/author,title,poemcount/{authorQuery};{titleQuery};{_batchSize}/author,title,lines");
            message.Headers.Remove("Accept");
            message.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
            var poems = await response.Content.ReadFromJsonAsync<List<PoetryDbPoem>>() 
                ?? throw new NullReferenceException("Deserialized poems list is null.");

            return poems.Select(p => new PoetryDbPoemLinesMerged
            {
                Title = p.Title,
                Author = p.Author,
                LinesMerged = string.Join("\n", p.Lines)
            }).ToList();
        }

    }
}