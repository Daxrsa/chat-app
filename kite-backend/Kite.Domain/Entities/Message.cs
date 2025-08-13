using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Message : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset SentAt { get; set; }
    public bool IsRead { get; set; }
    public virtual ICollection<ApplicationFile> Files { get; set; } = [];
    
    public virtual Conversation Conversation { get; set; } = null!;
    public virtual ApplicationUser Sender { get; set; } = null!;
}