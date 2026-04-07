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


        [HttpGet("search/{title}/{author}")]
        public async Task<ActionResult> PreparePagesAsync(string title, string author)
        {
            await _orchestration.PreparePagesAsync(title, author);
            return Ok();
        }

        [HttpGet("page/{page:int}")]
        public ActionResult GetPage(int page)
        {
            var poems = _orchestration.GetPage(page);
            if (poems is null) return NotFound();
            return Ok(new GetPageResponse { PoemMetadata = [.. poems] });
        }

        [HttpGet("poem/{poemId:int}/{languageId:int}")]
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
            public required PersistedPoem Poem { get; set; }
        }

    }
}