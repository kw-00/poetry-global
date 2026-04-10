using Microsoft.AspNetCore.Mvc;

namespace PoetryGlobal.Features.Poems
{


    [Route("api/[controller]")]
    [ApiController]
    public class PoemController : ControllerBase
    {
        private readonly IPoemOrchestration _orchestration;

        public PoemController(IPoemOrchestration orchestration)
        {
            _orchestration = orchestration;
        }


        [HttpGet("search")]
        public async Task<ActionResult> Search(string title, string author, int page)
        {
            var query = new SearchQueryDTO(title, author);
            var result = await _orchestration.SearchAsync(query, page);
            return Ok(new GetPageResponse { PoemMetadata = result.Page });
        }

        [HttpGet("{poemId:int}/{languageId:int}")]
        public async Task<ActionResult> GetPoemAsync(int poemId, int languageId)
        {
            var poem = await _orchestration.GetPoemAsync(poemId, languageId);
            if (poem is null) return NotFound();
            return Ok(new GetPoemResponse { Poem = poem });
        }


        internal class GetPageResponse
        {
            public required List<PersistedPoemMetadata> PoemMetadata { get; set; }
        }

        internal class GetPoemResponse
        {
            public required PoemDTO Poem { get; set; }
        }

    }
}