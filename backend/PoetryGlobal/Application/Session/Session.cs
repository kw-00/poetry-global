
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PoetryGlobal.Session
{
    public class CurrentSession
    {
        public required Guid Guid { get; init; }

        public CurrentSession(HttpContextAccessor _httpContextAccessor)
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
                throw new ValidationException("Session GUID claim is missing or invalid.");
            }

            Guid = sessionGuid;
        }
    }
}