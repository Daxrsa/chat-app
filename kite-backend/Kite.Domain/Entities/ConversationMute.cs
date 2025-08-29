using Kite.Domain.Enums;
using Kite.Domain.ValueObjects;

namespace Kite.Domain.Entities;

public class ConversationMute
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string MutedByUserId { get; set; } = string.Empty;
    public MuteReason Reason { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}