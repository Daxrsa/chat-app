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
    private readonly ITokenService _tokenService;
    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<Result<UserModel>> RegisterAsync(RegisterModel model)
    {
        if (model == null)
        {
            return Result<UserModel>.Failure(new Error("Registration.NullModel",
                "Registration model cannot be null"));
        }
        
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return Result<UserModel>.Failure(new Error("Registration.DuplicateEmail",
                "User with this email already exists"));
        }

        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray();
            return Result<UserModel>.Failure(errors);
        }
        await _userManager.AddToRoleAsync(user, "User");
        var userModel = new UserModel
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName
        };

        return Result<UserModel>.Success(userModel);
    }

    public async Task<Result<UserModel>> LoginAsync(LoginModel model)
    {
        if (model == null)
        {
            return Result<UserModel>.Failure(new Error("Login.NullModel",
                "Login model cannot be null"));
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Result<UserModel>.Failure(new Error("Login.InvalidCredentials",
                "Invalid email or password"));
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password,
            model.RememberMe, false);

        if (!result.Succeeded)
        {
            return Result<UserModel>.Failure(new Error("Login.Failed",
                "Invalid email or password"));
        }
        
        var token = await _tokenService.GenerateJwtTokenAsync(user);
        
        var userModel = new UserModel
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Token = token
        };

        return Result<UserModel>.Success(userModel);
    }

    public async Task<bool> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return true;
    }

    public async Task<Result<string>> DeleteUserByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return Result<string>.Failure(new Error("DeleteUser.InvalidEmail",
                "Email cannot be null or empty"));
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<string>.Failure(new Error("DeleteUser.UserNotFound",
                "User with this email does not exist"));
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray();
            return Result<string>.Failure(errors);
        }

        return Result<string>.Success($"User with email {email} has been successfully deleted");
    }
}