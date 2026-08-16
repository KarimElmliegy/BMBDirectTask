using System.Security.Claims;
using BMBAssessment.Application.Exceptions;
using BMBAssessment.Application.Interfaces;

namespace BMBAssessment.API.Services;
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
    public int CustomerId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
            return int.TryParse(value, out var id) ? id : throw new UnauthorizedException("The access token has no valid customer identifier.");
        }
    }
}
