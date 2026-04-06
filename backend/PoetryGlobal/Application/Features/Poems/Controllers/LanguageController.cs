using Microsoft.AspNetCore.Mvc;
namespace PoetryGlobal.Features.Poems
{


    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguagesService _languageService;

        public LanguageController(ILanguagesService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Language>>> GetAllLanguagesAsync()
        {
            try
            {
                var languages = await _languageService.GetAllLanguagesAsync();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error:{ex.Message}");
            }

        }
    }
}