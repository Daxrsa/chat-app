using System.Security.Claims;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserAccessor(
        IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    private ClaimsPrincipal GetCurrentClaimsPrincipal()
    {
        return _httpContextAccessor.HttpContext?.User;
    }

    public async Task<Result<UserModel>> GetCurrentUserAsync()
    {
        try
        {
            var principal = GetCurrentClaimsPrincipal();
            if (principal == null)
            {
                return Result<UserModel>.Failure(UserErrors.NoPrincipal);
            }
        
            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                return Result<UserModel>.Failure(UserErrors.NotFound);
            }

            var roles = await _userManager.GetRolesAsync(user);
            
            var userModel = new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? "None",
                ImageUrl = user.ImageUrl,
                CreatedAt = user.CreatedAt
            };
        
            return Result<UserModel>.Success(userModel);
        }
        catch (Exception ex)
        {
            return Result<UserModel>.Failure(
                new Error("Authentication.Exception", $"An error occurred while getting the current user: {ex.Message}"));
        }
    }
    
    public async Task<Result<string>> GetCurrentUserIdAsync()
    {
        try
        {
            var principal = GetCurrentClaimsPrincipal();
            if (principal == null)
            {
                return Result<string>.Failure(UserErrors.NoPrincipal);
            }
            
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                return Result<string>.Success(userId);
            }
            
            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                return Result<string>.Failure(UserErrors.NotFound);
            }
            
            return Result<string>.Success(user.Id);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("Authentication.Exception", $"An error occurred while getting the current user ID: {ex.Message}"));
        }
    }
}