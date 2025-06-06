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
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager) : IUserAccessor
{
    private ClaimsPrincipal GetCurrentClaimsPrincipal()
    {
        return httpContextAccessor.HttpContext?.User;
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

            var user = await userManager.GetUserAsync(principal);
            if (user == null)
            {
                return Result<UserModel>.Failure(UserErrors.NotFound);
            }

            var roles = await userManager.GetRolesAsync(user);

            var userModel = new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? "None",
                CreatedAt = user.CreatedAt
            };

            return Result<UserModel>.Success(userModel);
        }
        catch (Exception ex)
        {
            return Result<UserModel>.Failure(
                new Error("Authentication.Exception",
                    $"An error occurred while getting the current user: {ex.Message}"));
        }
    }

    public string GetCurrentUserId()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
    
    public async Task<Result<List<UserModel>>> GetAllUsersAsync()
    {
        try
        {
            var applicationUsers = await userManager.Users.ToListAsync();
            var userModels = new List<UserModel>();
        
            foreach (var user in applicationUsers)
            {
                var roles = await userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? ""; 
                
                var userModel = new UserModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName, 
                    UserName = user.UserName,
                    LastName = user.LastName,  
                    Email = user.Email,
                    Token = "",
                    Role = role,
                    CreatedAt = user.CreatedAt
                };
                userModels.Add(userModel);
            }
            
            return Result<List<UserModel>>.Success(userModels);
        }
        catch (Exception ex)
        {
            return Result<List<UserModel>>.Failure(
                new Error("Authentication.Exception",
                    $"An error occurred while getting all users: {ex.Message}"));
        }
    }
    
    public string GetUserFirstName()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName);
    }
    
    public string GetUserLastName()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Surname);
    }
}