using Microsoft.AspNetCore.Mvc;
using PoetryGlobal.Domains.Poems.Service;

namespace PoetryGlobal.Domains.Poems.Controller
{


    [Route("api/[controller]")]
    [ApiController]
    public class PoemController : ControllerBase
    {
        private readonly IPoemService _poemService;

        public PoemController(IPoemService poemService)
        {
            _poemService = poemService;
        }

        [HttpGet("/id/{poemId:int}/language/{languageId:int}")]
        public async Task<ActionResult> GetPoem(int poemId, int languageId)
        {
            try
            {
                var responseDto = await _poemService.GetPoem(poemId, languageId);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error: {ex.Message}");
            }
        }

        [HttpGet("/title/{title:string}/author/{author:string}")]
        public async Task<ActionResult> SearchPoems(string title, string author)
        {
            try
            {
                var responseDto = await _poemService.SearchPoems(title, author);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error: {ex.Message}");
            }
        }
    }
}