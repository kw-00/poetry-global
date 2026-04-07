
using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Features.Poems
{
    public class PoemOrchestration(
        IPoemRepository poemRepository, 
        IPoetryDbService poetryDbService,
        IPoemSearchCache poemSearchCache,
        ITranslationService translationService,
        ILanguageCache languageCache

    ) : IPoemOrchestration
    {
        private readonly IPoemRepository _poemRepository = poemRepository;
        private readonly IPoetryDbService _poetryDbService = poetryDbService;
        private readonly IPoemSearchCache _poemSearchCache = poemSearchCache;
        private readonly ITranslationService _translationService = translationService;
        private readonly ILanguageCache _languageCache = languageCache;

        public async Task PreparePagesAsync(string titleQuery, string authorQuery)
        {
            var poemsFromPoetryDb = await _poetryDbService.GetPoemsAsync(titleQuery, authorQuery);
            await _poemRepository.SavePoemOriginalsAsync(poemsFromPoetryDb);

            var poemsFromDb = await _poemRepository.SearchPoemsAsync(titleQuery, authorQuery);
            _poemSearchCache.SetSearchCache(poemsFromDb);
        }

        public PersistedPoemMetadata[] GetPage(int page)
        {
            var cache = _poemSearchCache.GetSearchCache()
                ?? throw new NullReferenceException($"Cache not initialized for current session.");

            return cache.GetPage(page);
        }

        public async Task<PersistedPoem> GetPoemAsync(int poemId, int languageId)
        {
            var poemVersion = await _poemRepository.GetPoemVersionAsync(poemId, languageId);
            if (poemVersion is not null) return poemVersion;

            var original = await _poemRepository.GetPoemOriginalAsync(poemId) 
                ?? throw new ModelNotPersistedException("Original poem could not be found.");

            var sourceLanguage = await _languageCache.GetLanguageCodeAsync(original.LanguageId) 
                ?? throw new NotFoundException($"Language with ID of {original.LanguageId} was not found.");
            var targetLanguage = await _languageCache.GetLanguageCodeAsync(languageId) 
                ?? throw new NotFoundException($"Language with ID of {languageId} was not found.");
            var translatedText = await _translationService.TranslatePoemAsync(original.VersionText, sourceLanguage, targetLanguage);
                
            var translatedPoem = new Poem
            {
                Title = original.Title,
                Author = original.Author,
                LanguageId = languageId,
                IsOriginal = false,
                VersionText = translatedText
            };

            return await _poemRepository.SavePoemVersionAsync(translatedPoem) 
                ?? throw new ModelNotPersistedException("Poem version could not be persisted.");
        }
    }
}