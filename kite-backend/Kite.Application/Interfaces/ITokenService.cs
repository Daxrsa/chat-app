using Kite.Domain.Common;
using Kite.Domain.Entities;

namespace Kite.Application.Interfaces;

public interface ITokenService
{
    Result<string> CreateToken(ApplicationUser user);
}