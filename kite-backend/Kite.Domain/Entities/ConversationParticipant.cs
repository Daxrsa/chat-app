using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class ConversationParticipant : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? LastReadAt { get; set; }
    public bool IsModerator { get; set; }
    
    public virtual Conversation Conversation { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}