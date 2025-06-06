using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class ApplicationFile : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public FileType Type { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } 
    public DateTimeOffset UploadedAt { get; set; }
}