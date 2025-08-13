using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Conversation : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Optional for group chats
    public DateTimeOffset CreatedAt { get; set; }
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = [];
    public virtual ICollection<Message> Messages { get; set; } = [];
}