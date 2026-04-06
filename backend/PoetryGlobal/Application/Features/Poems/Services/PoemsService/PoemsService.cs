
namespace PoetryGlobal.Features.Poems
{
    public class PoemsService(IDatabaseService databaseService, IPoetryDbService poetryDbService) : IPoemsService
    {
        private readonly IDatabaseService _databaseService = databaseService;
        private readonly IPoetryDbService _poetryDbService = poetryDbService;

        public async Task<List<PoemMetadataWithId>> SearchPoemsAsync(string titleQuery, string authorQuery)
        {
            return await _databaseService.SearchPoemsAsync(titleQuery, authorQuery);
        }

        public async Task<PoemVersionWithId> GetPoemAsync(int poemId, int languageId)
        {
            throw new NotImplementedException();
        }
    }
}