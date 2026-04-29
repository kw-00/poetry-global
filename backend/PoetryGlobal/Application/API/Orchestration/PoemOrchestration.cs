
using PoetryGlobal.Config;
using PoetryGlobal.Exceptions;
using PoetryGlobal.Infrastructure;


namespace PoetryGlobal.API
{
    public class PoemOrchestration(
        IPoemMetadataRepository poemMetadataRepository,
        IPoemVersionRepository poemRepository, 
        IExternalPoetryAPIService poetryDbService,
        IPoemSearchCache poemSearchCache,
        ITranslationService translationService,
        ILanguageCache languageCache,

        IConfigWithValidation configuration,
        ILogger<PoemOrchestration> logger

    ) : IPoemOrchestration
    {
        private readonly IPoemMetadataRepository _poemMetadataRepository 
            = poemMetadataRepository;
        private readonly IPoemVersionRepository _poemVersionRepository 
            = poemRepository;
        private readonly IExternalPoetryAPIService _poetryDbService 
            = poetryDbService;
        private readonly IPoemSearchCache _poemSearchCache 
            = poemSearchCache;
        private readonly ITranslationService _translationService 
            = translationService;

        private readonly ILanguageCache _languageCache = languageCache;

        private readonly int _databaseSearchBatchSize 
            = configuration.GetFromConfigOrThrow<int>(
                "Features:Poems:Search:DatabaseBatchSize"
            );
        private readonly int _poetryDbSearchBatchSize
            = configuration.GetFromConfigOrThrow<int>(
                "Features:Poems:Search:ExternalPoetryAPIBatchSize"
            );

        private readonly int _preferredMinBatchSize
            = configuration.GetFromConfigOrThrow<int>(
                "Features:Poems:Search:PreferredMinBatchSize"
            );

        private readonly double _minSuccessfulSaveRatio
            = configuration.GetFromConfigOrThrow<double>("Features:Poems:Search:MinSuccessfulSaveRatio");
        private readonly int _pageSize
            = configuration.GetFromConfigOrThrow<int>("Features:Poems:Search:PageSize");

        public async Task<(int PageCount, IReadOnlyList<PersistedPoemMetadata> Page)> SearchAsync(
            SearchQueryDTO query, int pageNumber
        )
        {
            // Local helper function for caching search results and getting a page to return
            (int PageCount, IReadOnlyList<PersistedPoemMetadata> Page) CacheResultsAndGetPage(
                List<PersistedPoemMetadata> metadata
            )
            {
                var pages = new SearchPages(metadata, _pageSize);
                _poemSearchCache.Set(query, pages);
                var page = pages.GetPage(pageNumber) 
                    ?? throw new NotFoundException("Page number out of bounds.");
                return (pages.PageCount, [.. page]);
            }
            logger.LogCritical("Search query: {Query}", (query.Title, query.Author));
            var cachedPages = _poemSearchCache.Get(query);
            if (cachedPages is not null)
            {
                var page = cachedPages.GetPage(pageNumber) 
                    ?? throw new NotFoundException("Page number out of bounds.");
                return (cachedPages.PageCount, [.. page]);
            }

            // If database returns enough poem metadata records, use them and return
            var metadataFromDatabase = await _poemMetadataRepository
                .SearchAsync(query, _databaseSearchBatchSize);
            if (metadataFromDatabase.Count >= _preferredMinBatchSize)
            {
                return CacheResultsAndGetPage(metadataFromDatabase);
            }

            // If database returns too few records, check external API for poem metadata
            var metadataFromExternalAPI = await _poetryDbService
                .GetPoemMetadataAsync(query, _poetryDbSearchBatchSize);

            var totalPoemCount = metadataFromExternalAPI
                .UnionBy(metadataFromDatabase.Select(m => m.ToNormalModel()), m => m.Id)
                .Count();
            var newPoemCount = metadataFromExternalAPI
                .ExceptBy(metadataFromDatabase.Select(m => m.ToNormalModel().Id), m => m.Id)
                .Count();
            
            // Check if external API found enough poems that were not in database.
            // Let N mean "number of poems found  in external API" 
            // and let Total mean "total number of poems found 
            // in external API and database", deduplicated.
            // If N / Total < threshold, ignore external API results 
            // and use what was found in database.
            // If N / Total >= threshold, retrieve full poem texts 
            // from external API and save them to database,
            // then use both records from external API and from database.
            var potentialSuccessfulSaveRatio 
                = totalPoemCount > 0 
                    ? newPoemCount / totalPoemCount 
                    : 0;

            if (potentialSuccessfulSaveRatio >= _minSuccessfulSaveRatio)
            {
                var metadataAndVersions = await _poetryDbService
                    .GetPoemsAsync(query, _poetryDbSearchBatchSize);
                var metadata = metadataAndVersions.Select(m => m.PoemMetadata).ToList();
                var versions = metadataAndVersions.Select(m => m.PoemVersion).ToList();

                var persistedMetadata = await _poemMetadataRepository.UpsertAllAsync(metadata);
                var versionsToUpsert = persistedMetadata
                    .Zip(versions, (m, v) => (Metadata: m, Version: v))
                    .Select(mNv => new PoemVersion
                    {
                        PoemMetadataId = mNv.Metadata.Id,
                        LanguageId = mNv.Version.LanguageId,
                        IsOriginal = mNv.Version.IsOriginal,
                        VersionText = mNv.Version.VersionText
                    }).ToList();

                await _poemVersionRepository.UpsertAllAsync(versionsToUpsert);

                return CacheResultsAndGetPage(persistedMetadata);
            }
            // If external API returns too few results that were not in database, 
            // use database search results and return
            else
            {
                return CacheResultsAndGetPage(metadataFromDatabase);
            }
        }

        public async Task<PoemDTO> GetPoemAsync(int poemMetadataId, int languageId)
        {
            var poemMetadata = await _poemMetadataRepository.GetAsync(poemMetadataId)
                ?? throw new NotFoundException(
                    $"Poem metadata ID of {poemMetadataId} was not found in database."
                );

            // If suitable language version already exists, return it
            var poemVersion = await _poemVersionRepository.GetAsync(poemMetadataId, languageId);
            if (poemVersion is not null) return new PoemDTO(poemMetadata, poemVersion);

            var originalVersion = await _poemVersionRepository.GetOriginalAsync(poemMetadataId);
            if (originalVersion is null)
            {
                await _poemMetadataRepository.DeleteAsync(poemMetadataId);
                throw new NotFoundException(
                    "Original poem could not be found. Corresponding poem metadata"
                    + " has been removed, as poem metadata should not exist"
                    + " without corresponding original poem version."
                );
            }

            var sourceLanguage = await _languageCache
                .GetLanguageCodeAsync(originalVersion.LanguageId)
                ?? throw new NotFoundException(
                    $"Language with ID of {originalVersion.LanguageId} was not found."
                );
            var targetLanguage = await _languageCache
                .GetLanguageCodeAsync(languageId)
                ?? throw new NotFoundException(
                    $"Language with ID of {languageId} was not found."
                );
            var translatedText = await _translationService.TranslatePoemAsync(
                originalVersion.VersionText, sourceLanguage, targetLanguage
            );
            
            // If no suitable version exists yet, translate the original
            var translatedVersion = new PoemVersion
            {
                PoemMetadataId = poemMetadataId,
                IsOriginal = false,
                LanguageId = languageId,
                VersionText = translatedText
            };
            var persistedTranslatedVersion = await _poemVersionRepository
                .UpsertAsync(translatedVersion);

            return new PoemDTO(poemMetadata, persistedTranslatedVersion);
        }


    }
}