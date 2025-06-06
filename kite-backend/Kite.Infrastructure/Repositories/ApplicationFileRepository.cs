using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class ApplicationFileRepository : GenericRepository<ApplicationFile, Guid>, IApplicationFileRepository
{
    public ApplicationFileRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<ApplicationFile>> GetUserFilesByTypeAsync(string userId, FileType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.UserId == userId && f.Type == type)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationFile?> GetLatestUserFileByTypeAsync(string userId, FileType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.UserId == userId && f.Type == type)
            .OrderByDescending(f => f.UploadedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}