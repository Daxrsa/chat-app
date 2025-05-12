using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;

namespace Kite.Application.Interfaces;

public interface IAuthService
{
    Task<Result<UserModel>> RegisterAsync(RegisterModel model);
    Task<Result<UserModel>>LoginAsync(LoginModel model);
    Task<bool> LogoutAsync();
    Task<Result<string>> DeleteUserByEmail(string email);
}