




using Microsoft.AspNetCore.Mvc;
using PoetryGlobal.Features.Auth;

namespace PoetryGlobal.Features.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("session-token")]
        public string GetSessionToken()
        {
            return _authService.GenerateJwtToken();
        }

    }
}