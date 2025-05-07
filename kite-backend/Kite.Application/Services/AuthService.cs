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
        // Check for null or empty values in request
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return Result<string>.Failure("Username, email, and password are required.");
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        try
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Log errors or return detailed errors
                var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                Console.WriteLine($"User creation failed: {errors}"); // For debugging
                return Result<string>.Failure(errors);
            }

            return Result<string>.Success("Registration successful.");
        }
        catch (Exception ex)
        {
            // Log the exception and return a friendly error
            Console.WriteLine($"Exception during user creation: {ex}"); // For debugging
            return Result<string>.Failure($"Exception: {ex.Message}");
        }
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