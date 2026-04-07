using Microsoft.AspNetCore.Mvc;

namespace PoetryGlobal.Features.Poems
{


    [Route("api/[controller]")]
    [ApiController]
    public class PoemController : ControllerBase
    {
        private readonly IPoemOrchestration _poemService;

        public PoemController(IPoemOrchestration poemService)
        {
            _poemService = poemService;
        }

        [HttpGet("/id/{poemId:int}/language/{languageId:int}")]
        public async Task<ActionResult> GetPoemAsync(int poemId, int languageId)
        {
            try
            {
                var poems = await _poemService.GetPoemAsync(poemId, languageId);
                return Ok(poems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error: {ex.Message}");
            }
        }


        [HttpGet("/database/title/{title:string}/author/{author:string}")]
        public async Task<ActionResult> SearchPoemsAsync(string title, string author)
        {
            try
            {
                var poemMetadata = await _poemService.SearchPoemsAsync(title, author);
                return Ok(poemMetadata);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error: {ex.Message}");
            }
        }
    }
}