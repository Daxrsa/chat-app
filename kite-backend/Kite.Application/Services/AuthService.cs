using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kite.Application.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IUserAccessor userAccessor,
    IApplicationFileRepository applicationFileRepository,
    ITokenService tokenService,
    IFileUrlService fileUrlService) : IAuthService
{
    public async Task<Result<UserModel>> RegisterAsync(RegisterModel model)
    {
        if (model == null)
        {
            return Result<UserModel>.Failure(new Error("Registration.NullModel",
                "Registration model cannot be null"));
        }

        var existingUser = await userManager.FindByEmailAsync(model.Email);
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
            LastName = model.LastName,
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray();
            return Result<UserModel>.Failure(errors);
        }

        await userManager.AddToRoleAsync(user, Role.User);
        var roles = await userManager.GetRolesAsync(user);
        var userModel = new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Role = roles.FirstOrDefault() ?? "None",
            CreatedAt = user.CreatedAt
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

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Result<UserModel>.Failure(new Error("Login.InvalidCredentials",
                "Invalid email or password"));
        }

        var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password,
            model.RememberMe, false);

        if (!result.Succeeded)
        {
            return Result<UserModel>.Failure(UserErrors.IncorrectEmailOrPassword);
        }

        var token = await tokenService.GenerateJwtTokenAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        var userModel = new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Token = token,
            Role = roles.FirstOrDefault() ?? "None",
            CreatedAt = user.CreatedAt
        };

        return Result<UserModel>.Success(userModel);
    }

    public async Task<Result<string>> DeleteUserByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return Result<string>.Failure(new Error("DeleteUser.InvalidEmail",
                "Email cannot be null or empty"));
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<string>.Failure(UserErrors.EmailNotFound(email));
        }

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray();
            return Result<string>.Failure(errors);
        }

        return Result<string>.Success($"User with email {email} has been successfully deleted");
    }

    public async Task<Result<List<UserModel>>> GetAllUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var applicationUsers = await userManager.Users.ToListAsync(cancellationToken);
        
        var userIds = applicationUsers.Select(u => u.Id).ToList();
        
        var profilePictures = await applicationFileRepository
            .GetLatestUserFilesByTypeAsync(userIds, FileType.ProfilePicture, cancellationToken);
        
        var profilePictureDict = profilePictures.ToDictionary(p => p.UserId, p => p.FilePath);

        var userRoles = new Dictionary<string, string>();
        foreach (var user in applicationUsers)
        {
            var roles = await userManager.GetRolesAsync(user);
            userRoles[user.Id] = roles.FirstOrDefault() ?? "";
        }
        
        var userModels = await Task.WhenAll(
            applicationUsers.Select(async user => new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                UserName = user.UserName ?? string.Empty,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = userRoles[user.Id],
                ProfilePicture = fileUrlService.ServeFileUrl(profilePictureDict.GetValueOrDefault(user.Id) ?? string.Empty),
                CreatedAt = user.CreatedAt
            })
        );

        var userModelsList = userModels.ToList();

        return Result<List<UserModel>>.Success(userModelsList);
    }

    public async Task<Result<UserModel>> GetCurrentUserAsync(
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<UserModel>.Failure(
                new Error("Auth.NotAuthenticated", "User is not authenticated."));
        }

        var user = await userManager.FindByIdAsync(currentUserId);
        if (user == null)
        {
            return Result<UserModel>.Failure(
                new Error("User.NotFound", "User account not found."));
        }

        var role = await userManager.GetRolesAsync(user);
        var profilePicture =
            await applicationFileRepository.GetLatestUserFileByTypeAsync(currentUserId,
                FileType.ProfilePicture, cancellationToken);

        var userModel = new UserModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            UserName = user.UserName ?? string.Empty,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            ProfilePicture = fileUrlService.ServeFileUrl(profilePicture?.FilePath ?? string.Empty),
            Role = role.FirstOrDefault() ?? string.Empty
        };

        return Result<UserModel>.Success(userModel);
    }
}