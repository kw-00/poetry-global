using Microsoft.AspNetCore.Mvc;
using PoetryGlobal.Domains.Languages.Service;
using PoetryGlobal.Domains.Languages.Models;

namespace PoetryGlobal.Domains.Languages.Controller
{


    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Language>>> GetAllLanguages()
        {
            try
            {
                var languages = await _languageService.GetAllLanguages();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request. Error:{ex.Message}");
            }

        }
    }
}