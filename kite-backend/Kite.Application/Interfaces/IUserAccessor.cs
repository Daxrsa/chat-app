using System.Security.Claims;
using Kite.Domain.Entities;

namespace Kite.Application.Interfaces;

public interface IUserAccessor
{
    Task<ApplicationUser> GetCurrentUserAsync();
}