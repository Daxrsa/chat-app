using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class UserRepository : GenericRepository<ApplicationUser, string>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}