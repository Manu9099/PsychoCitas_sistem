using System.Security.Claims;
using PsychoCitas.Application.Common.Interfaces;

namespace PsychoCitas.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var guid)
                ? guid
                : Guid.Empty;
        }
    }

    public string Email =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public string Rol =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}