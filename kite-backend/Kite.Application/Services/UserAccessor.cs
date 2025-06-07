using System.Security.Claims;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kite.Application.Services;

public class UserAccessor(
    IHttpContextAccessor httpContextAccessor) : IUserAccessor
{
    public string? GetCurrentUserId()
    {
        var user = GetCurrentUser();
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? user?.FindFirst("sub")?.Value
               ?? user?.FindFirst("id")?.Value;
    }

    public string? GetCurrentUserName()
    {
        var user = GetCurrentUser();
        return user?.FindFirst(ClaimTypes.Name)?.Value
               ?? user?.FindFirst("username")?.Value;
    }

    public string? GetCurrentUserFirstName()
    {
        var user = GetCurrentUser();
        return user?.FindFirst(ClaimTypes.GivenName)?.Value
               ?? user?.FindFirst("firstname")?.Value;
    }

    public string? GetCurrentUserLastName()
    {
        var user = GetCurrentUser();
        return user?.FindFirst(ClaimTypes.Surname)?.Value
               ?? user?.FindFirst("lastname")?.Value;
    }

    public string? GetCurrentUserEmail()
    {
        var user = GetCurrentUser();
        return user?.FindFirst(ClaimTypes.Email)?.Value
               ?? user?.FindFirst("email")?.Value;
    }

    public ClaimsPrincipal? GetCurrentUser()
    {
        return httpContextAccessor.HttpContext?.User;
    }

    public bool IsAuthenticated()
    {
        var user = GetCurrentUser();
        return user?.Identity?.IsAuthenticated ?? false;
    }

    public bool HasClaim(string claimType, string claimValue)
    {
        var user = GetCurrentUser();
        return user?.HasClaim(claimType, claimValue) ?? false;
    }

    public IEnumerable<string> GetUserRoles()
    {
        var user = GetCurrentUser();
        if (user == null) return Enumerable.Empty<string>();

        return user.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Union(user.FindAll("role").Select(c => c.Value))
            .Distinct();
    }

    public bool IsInRole(string role)
    {
        var user = GetCurrentUser();
        return user?.IsInRole(role) ?? false;
    }

    public string? GetClaimValue(string claimType)
    {
        var user = GetCurrentUser();
        return user?.FindFirst(claimType)?.Value;
    }

    public Dictionary<string, string> GetAllClaims()
    {
        var user = GetCurrentUser();
        if (user == null) return new Dictionary<string, string>();

        return user.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.First().Value);
    }
}