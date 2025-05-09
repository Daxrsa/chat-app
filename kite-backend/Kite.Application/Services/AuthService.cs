using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<Result<string>> Register(RegisterModel model)
    {
        if (model == null)
        {
            return Result<string>.Failure("Registration model cannot be null.");
        }
        
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };
        
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Result<string>.Success("User created successfully.");
        }
        
        var errors = result.Errors.Select(e => new Error("Identity.Error", e.Description)).ToArray();
        return Result<string>.Failure(errors);
    }

    public async Task<Result<string>> DeleteUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<string>.Failure("Email cannot be null or empty.");
        }
        
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<string>.Failure("User with the specified email does not exist.");
        }
        
        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return Result<string>.Success($"User with email {email} has been deleted successfully.");
        }
        
        var errors = result.Errors.Select(e => new Error("Identity.Error", e.Description)).ToArray();
        return Result<string>.Failure(errors);
    }
}