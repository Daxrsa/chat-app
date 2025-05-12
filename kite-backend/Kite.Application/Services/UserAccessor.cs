using System.Security.Claims;
using Kite.Application.Interfaces;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserAccessor(
        IHttpContextAccessor httpContextAccessor, 
        UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public ClaimsPrincipal GetCurrentClaimsPrincipal()
    {
        return _httpContextAccessor.HttpContext?.User;
    }

    public string GetCurrentUserId()
    {
        var principal = GetCurrentClaimsPrincipal();
        return principal != null ? _userManager.GetUserId(principal) : null;
    }

    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var principal = GetCurrentClaimsPrincipal();
        return principal != null ? await _userManager.GetUserAsync(principal) : null;
    }

}