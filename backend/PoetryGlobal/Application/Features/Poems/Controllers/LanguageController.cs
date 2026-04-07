using Microsoft.AspNetCore.Mvc;
namespace PoetryGlobal.Features.Poems
{


    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageRepository _languageService;

        public LanguageController(ILanguageRepository languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Language>>> GetAllLanguagesAsync()
        {
            var languages = await _languageService.GetAllLanguagesAsync();
            return Ok(new GetAllLanguagesResponse { languages = languages }); 
        }

        internal class GetAllLanguagesResponse
        {
            public required List<PersistedLanguage> languages { get; set; }
        }
    }
}