
using System.Security.Claims;
using PoetryGlobal.Exceptions;

namespace PoetryGlobal.Session
{
    public class CurrentSession : ICurrentSession
    {
        public required Guid Guid { get; init; }

        public CurrentSession(IHttpContextAccessor _httpContextAccessor)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                throw new NullReferenceException("HttpContext is not available.");
            }

            var sessionGuidClaim =
                httpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (sessionGuidClaim is null || !Guid.TryParse(sessionGuidClaim, out var sessionGuid))
            {
                throw new UnauthorizedException("Session GUID claim is missing or invalid.");
            }

            Guid = sessionGuid;
        }
    }
}