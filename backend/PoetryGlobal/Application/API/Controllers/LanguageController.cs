using Microsoft.AspNetCore.Mvc;
using PoetryGlobal.Infrastructure;
namespace PoetryGlobal.API
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
        public async Task<ActionResult<IReadOnlyList<PersistedLanguage>>> GetAllLanguagesAsync()
        {
            var languages = await _languageService.GetAllLanguagesAsync();
            return Ok(new GetAllLanguagesResponse { Languages = languages }); 
        }

        internal class GetAllLanguagesResponse
        {
            public required IReadOnlyList<PersistedLanguage> Languages { get; set; }
        }
    }
}