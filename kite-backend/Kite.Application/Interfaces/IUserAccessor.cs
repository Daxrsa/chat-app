using System.Security.Claims;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;

namespace Kite.Application.Interfaces;

public interface IUserAccessor
{
    Task<Result<UserModel>> GetCurrentUserAsync();
    Task<Result<string>> GetCurrentUserIdAsync();
}