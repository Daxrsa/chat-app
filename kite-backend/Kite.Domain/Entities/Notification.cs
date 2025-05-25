using System.ComponentModel.DataAnnotations.Schema;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Notification : IEntity<Guid>
{
    public Guid Id { get; set; }
    public required string RecipientId { get; set; }  
    public virtual ApplicationUser Recipient { get; set; }
    public string? Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    [NotMapped]
    public DateTimeOffset CreatedAt { get; set; } 
    [NotMapped]
    public DateTimeOffset? ReadAt { get; set; }
}