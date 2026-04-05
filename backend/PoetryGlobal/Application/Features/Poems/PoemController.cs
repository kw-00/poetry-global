using Microsoft.AspNetCore.Mvc;
using PoetryGlobal.Features.Poems;

namespace PoetryGlobal.Features.Poems
{


    [Route("api/[controller]")]
    [ApiController]
    public class PoemController : ControllerBase
    {
        private readonly IPoemsService _poemService;

        public PoemController(IPoemsService poemService)
        {
            _poemService = poemService;
        }

        [HttpGet("/id/{poemId:int}/language/{languageId:int}")]
        public async Task<ActionResult> GetPoemAsync(int poemId, int languageId)
        {
            try
            {
                var responseDto = await _poemService.GetPoemAsync(poemId, languageId);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error: {ex.Message}");
            }
        }


        [HttpGet("/database/title/{title:string}/author/{author:string}")]
        public async Task<ActionResult> SearchDatabaseForPoemsAsync(string title, string author)
        {
            try
            {
                var responseDto = await _poemService.DatabaseSearchAsync(title, author);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error: {ex.Message}");
            }
        }

        [HttpGet("/poetrydb/title/{title:string}/author/{author:string}")]
        public async Task<ActionResult> SearchPoetryDbForPoemsAsync(string titleQuery, string authorQuery)
        {
            try
            {
                var responseDto = await _poemService.PoetryDbSearchAsync(titleQuery, authorQuery);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error: {ex.Message}");
            }
        }
    }
}