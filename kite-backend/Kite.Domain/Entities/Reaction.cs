using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Domain.Entities;

public class Reaction : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; } // The ID of the reacted entity (Post, Comment, or Reply)
    public EntityType EntityType { get; set; }
    public ReactionType ReactionType { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}