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

    public async Task<ApplicationUser> GetUserWithProfilePictureAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Files.Where(f => f.Type == FileType.ProfilePicture)
                .OrderByDescending(f => f.UploadedAt)
                .Take(1))
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}