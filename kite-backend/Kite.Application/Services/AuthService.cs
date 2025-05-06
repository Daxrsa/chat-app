using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result<string>> LoginAsync(LoginModel request)
    {
        ApplicationUser user = null;
        
        if (!string.IsNullOrEmpty(request.Email))
        {
            user = await _userManager.FindByEmailAsync(request.Email);
        }
        else if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
        }

        if (user == null)
        {
            return Result<string>.Failure("Invalid login credentials.");
        }
        
        var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, isPersistent: false, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return Result<string>.Failure("Invalid login credentials.");
        }

        return Result<string>.Success("Login successful.");
    }

    public async Task<Result<string>> RegisterAsync(RegisterModel request)
    {
        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Result<string>.Failure(string.Join("; ", result.Errors));
        }

        return Result<string>.Success("Registration successful.");
    }
    
    public async Task<Result<bool>> DeleteUserByEmailAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<bool>.Failure(UserErrors.NotFound(email));
            }

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return Result<bool>.Failure(UserErrors.DeletionUnexpectedError);
            }

            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(new Error("User.Exception", ex.Message));
        }
    }
}