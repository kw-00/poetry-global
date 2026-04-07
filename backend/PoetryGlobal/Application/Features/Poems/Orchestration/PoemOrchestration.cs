
namespace PoetryGlobal.Features.Poems
{
    public class PoemOrchestration(IPoemRepository databaseService, IPoetryDbService poetryDbService) : IPoemOrchestration
    {
        private readonly IPoemRepository _databaseService = databaseService;
        private readonly IPoetryDbService _poetryDbService = poetryDbService;

        public async Task<List<PersistedPoemMetadata>> SearchPoemsAsync(string titleQuery, string authorQuery)
        {
            return await _databaseService.SearchPoemsAsync(titleQuery, authorQuery);
        }

        public async Task<PersistedPoem> GetPoemAsync(int poemId, int languageId)
        {
            throw new NotImplementedException();
        }
    }
}