
using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Features.Poems
{
    public class PoemOrchestration(
        IPoemMetadataRepository poemMetadataRepository,
        IPoemVersionRepository poemRepository, 
        IPoetryDbService poetryDbService,
        IScopedPoemSearchCacheProvider poemSearchCache,
        ITranslationService translationService,
        ILanguageCache languageCache,

        IConfiguration configuration

    ) : IPoemOrchestration
    {
        private readonly IPoemMetadataRepository _poemMetadataRepository = poemMetadataRepository;
        private readonly IPoemVersionRepository _poemVersionRepository = poemRepository;
        private readonly IPoetryDbService _poetryDbService = poetryDbService;
        private readonly IScopedPoemSearchCacheProvider _poemSearchCache = poemSearchCache;
        private readonly ITranslationService _translationService = translationService;
        private readonly ILanguageCache _languageCache = languageCache;

        private static readonly string _databaseSearchBatchSizeKey = "Features:Poems:DatabaseSearchBatchSize";
        private readonly int _databaseSearchBatchSize = int.Parse(
            configuration[_databaseSearchBatchSizeKey]
                ?? throw new AppSettingsKeyNotFoundException(_databaseSearchBatchSizeKey)
        );

        private static readonly string _poetryDbSearchBatchSizeKey = "Features:Poems:PoetryDbSearchBatchSize";  
        private readonly int _poetryDbSearchBatchSize = int.Parse(
            configuration[_poetryDbSearchBatchSizeKey]
                ?? throw new AppSettingsKeyNotFoundException(_poetryDbSearchBatchSizeKey)
        );

        public async Task PreparePagesAsync(string titleQuery, string authorQuery)
        {
            List<PersistedPoemMetadata> metadataFromDatabaseSearch 
                = await _poemMetadataRepository.SearchAsync(
                    titleQuery, authorQuery, _databaseSearchBatchSize
                );

            List<PersistedPoemMetadata> metadataFromPoetryDbPersisted;

            {
                var dataFromPoetryDb = await _poetryDbService.GetPoemsAsync(
                    titleQuery, authorQuery, _poetryDbSearchBatchSize
                );
                var metadataPoetryDb = dataFromPoetryDb.Select(p => p.PoemMetadata).ToList();
                var versionsPoetryDb = dataFromPoetryDb.Select(p => p.PoemVersion).ToList();
                var persistedMetadata = await _poemMetadataRepository.UpsertAllAsync(metadataPoetryDb);

                var englishLanguageId = await _languageCache.GetLanguageIdAsync("en");
                List<PoemVersion> versionsToBePersisted = [..
                    persistedMetadata
                    .Zip(versionsPoetryDb, (m, v) => (PoemMetadata: m, PoemVersion: v))
                    .Select(tuple => new PoemVersion
                    {
                        PoemMetadataId = tuple.PoemMetadata.Id,
                        IsOriginal = true,
                        LanguageId = englishLanguageId,
                        VersionText = tuple.PoemVersion.VersionText
                    })
                ];
                await _poemVersionRepository.UpsertAllAsync(versionsToBePersisted);

                metadataFromPoetryDbPersisted = persistedMetadata;
            }

            _poemSearchCache.SetSearchCache([.. 
                metadataFromDatabaseSearch.UnionBy(metadataFromPoetryDbPersisted, m => m.Id)
            ]);
        }

        public PersistedPoemMetadata[] GetPage(int page)
        {
            var cache = _poemSearchCache.GetSearchCache()
                ?? throw new NotFoundException($"Cache not initialized for current session.");

            return cache.GetPage(page)
                ?? throw new NotFoundException($"Page {page} out of bounds.");
        }

        public async Task<PoemDTO> GetPoemAsync(int poemMetadataId, int languageId)
        {
            var poemMetadata = await _poemMetadataRepository.GetAsync(poemMetadataId)
                ?? throw new NotFoundException($"Poem metadata with ID of {poemMetadataId} was not found.");
            var poemVersion = await _poemVersionRepository.GetAsync(poemMetadataId, languageId);
            if (poemVersion is not null) return new PoemDTO(poemMetadata, poemVersion);

            var originalVersion = await _poemVersionRepository.GetOriginalAsync(poemMetadataId) 
                ?? throw new ModelNotPersistableException("Original poem could not be found.");

            var sourceLanguage = await _languageCache.GetLanguageCodeAsync(originalVersion.LanguageId) 
                ?? throw new NotFoundException($"Language with ID of {originalVersion.LanguageId} was not found.");
            var targetLanguage = await _languageCache.GetLanguageCodeAsync(languageId) 
                ?? throw new NotFoundException($"Language with ID of {languageId} was not found.");
            var translatedText = await _translationService.TranslatePoemAsync(
                originalVersion.VersionText, sourceLanguage, targetLanguage
            );
                
            var translatedVersion = new PoemVersion
            {
                PoemMetadataId = poemMetadataId,
                IsOriginal = false,
                LanguageId = languageId,
                VersionText = translatedText
            };
            var persistedTranslatedVersion = await _poemVersionRepository.UpsertAsync(translatedVersion);

            return new PoemDTO(poemMetadata, persistedTranslatedVersion);
        }
    }
}