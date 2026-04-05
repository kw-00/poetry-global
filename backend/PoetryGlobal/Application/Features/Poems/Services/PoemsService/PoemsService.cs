



using Npgsql;
using PoetryGlobal.Features.Poems;
using PoetryGlobal.Features.Poems;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{
    public class PoemsService(IDatabaseService databaseService, IPoetryDbService poetryDbService) : IPoemsService
    {
        private readonly IDatabaseService _databaseService = databaseService;
        private readonly IPoetryDbService _poetryDbService = poetryDbService;

        public async Task<List<PoemMetadataWithId>> DatabaseSearchAsync(string titleQuery, string authorQuery)
        {
            return await _databaseService.SearchForPoemsAsync(titleQuery, authorQuery);
        }

        public async Task<List<PoemMetadataWithId>> PoetryDbSearchAsync(string titleQuery, string authorQuery)
        {
            var poems = await _poetryDbService.GetPoemsAsync(titleQuery, authorQuery);
            var poemMetadata = await _databaseService.SavePoemOriginalsAsync(poems);
            return poemMetadata;
        }

        public async Task<PoemVersionWithId> GetPoemAsync(int poemId, int languageId)
        {
            throw new NotImplementedException();
        }
    }
}