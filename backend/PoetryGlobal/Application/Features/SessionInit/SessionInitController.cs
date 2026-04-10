




using Microsoft.AspNetCore.Mvc;
using PoetryGlobal.Exceptions;
using PoetryGlobal.Features.Auth;

namespace PoetryGlobal.Features.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionInitController(ITokenService tokenService, IConfiguration configuration) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _configuration = configuration;


        [HttpGet]
        public IActionResult GetSessionToken()
        {
            var tokenLifetimeKey = "Jwt:LifetimeMinutes";
            var lifetimeMinutes = int.Parse(
                _configuration[tokenLifetimeKey] 
                    ?? throw new AppSettingsKeyNotFoundException(tokenLifetimeKey)
            );
            var cookieOptions = new CookieOptions 
            { 
                HttpOnly = true, 
                Secure = true, 
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(lifetimeMinutes)
            };
            var token = _tokenService.GenerateJwtToken();
            Response.Cookies.Append("SessionToken", token, cookieOptions);
            return Ok();
        }

    }
}