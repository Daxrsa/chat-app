using System.ComponentModel.DataAnnotations.Schema;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Notification : IEntity<Guid>
{
    public Guid Id { get; set; }
    public required string ReceiverId { get; set; }  
    public virtual ApplicationUser Receiver { get; set; }
    public required string SenderId { get; set; }  
    public virtual ApplicationUser Sender { get; set; }
    
    public string? Title { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; } 
    public DateTimeOffset? ReadAt { get; set; }
}