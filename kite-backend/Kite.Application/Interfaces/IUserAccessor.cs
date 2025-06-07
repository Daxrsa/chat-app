using System.Security.Claims;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;

namespace Kite.Application.Interfaces;

public interface IUserAccessor
{
    string? GetCurrentUserId();
    string? GetCurrentUserName();
    string? GetCurrentUserEmail();
    string? GetCurrentUserFirstName();
    string? GetCurrentUserLastName();
    ClaimsPrincipal? GetCurrentUser();
    bool IsAuthenticated();
    bool HasClaim(string claimType, string claimValue);
    IEnumerable<string> GetUserRoles();
    bool IsInRole(string role);
    string? GetClaimValue(string claimType);
    Dictionary<string, string> GetAllClaims();
}