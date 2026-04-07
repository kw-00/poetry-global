




using Microsoft.AspNetCore.Mvc;
using PoetryGlobal.Features.Auth;

namespace PoetryGlobal.Features.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("session-token")]
        public IActionResult GetSessionToken()
        {
            return Ok(new GetSessionTokenResponse { Token = _authService.GenerateJwtToken() });
        }

    }

    internal class GetSessionTokenResponse
    {
        public required string Token { get; set; }    
    }
}