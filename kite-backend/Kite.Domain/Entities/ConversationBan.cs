using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class ConversationBan : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string BannedByUserId { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}