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
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    [NotMapped]
    public DateTimeOffset CreatedAt { get; set; } 
    [NotMapped]
    public DateTimeOffset? ReadAt { get; set; }
}