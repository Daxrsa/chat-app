using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IAuthService
{
    Task<Result<string>> Register(RegisterModel model);
    Task<Result<string>> DeleteUserByEmail(string email);
}